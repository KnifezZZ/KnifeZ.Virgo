using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using KnifeZ.Virgo.Core;
using KnifeZ.Virgo.Core.Auth;
using KnifeZ.Virgo.Core.Extensions;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using KnifeZ.Extensions.DatabaseAccessor;
using KnifeZ.Extensions;

namespace KnifeZ.Virgo.Mvc.Auth
{
    public class TokenService : ITokenService
    {
        private readonly ILogger _logger;
        private readonly JwtOption _jwtOptions;

        private const Token _emptyToken = null;

        private readonly Configs _configs;
        private readonly IDataContext _dc;
        public IDataContext DC => _dc;

        public TokenService(
            ILogger<TokenService> logger,
            IOptions<Configs> configs
        )
        {
            _configs = configs.Value;
            _jwtOptions = _configs.JwtOption;
            _logger = logger;
            _dc = CreateDC();
        }

        public async Task<Token> IssueTokenAsync(LoginUserInfo loginUserInfo)
        {
            if (loginUserInfo == null)
                throw new ArgumentNullException(nameof(loginUserInfo));

            var signinCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SecurityKey)), SecurityAlgorithms.HmacSha256);

            var tokeOptions = new JwtSecurityToken(
                issuer: _jwtOptions.Issuer,
                audience: _jwtOptions.Audience,
                claims: new List<Claim>()
                {
                    new Claim(AuthConstants.JwtClaimTypes.Subject, loginUserInfo.Id.ToString()),
                    new Claim(AuthConstants.JwtClaimTypes.Name, loginUserInfo.Name)
                },
                expires: DateTime.Now.AddSeconds(_jwtOptions.Expires),
                signingCredentials: signinCredentials
            );


            var refreshToken = new PersistedGrant()
            {
                UserId = loginUserInfo.Id,
                Type = "refresh_token",
                CreationTime = DateTime.Now,
                RefreshToken = Guid.NewGuid().ToString("N"),
                Expiration = DateTime.Now.AddSeconds(_jwtOptions.RefreshTokenExpires)
            };
            _dc.AddEntity(refreshToken);
            await _dc.SaveChangesAsync();

            return await Task.FromResult(new Token()
            {
                AccessToken = new JwtSecurityTokenHandler().WriteToken(tokeOptions),
                ExpiresIn = _jwtOptions.Expires,
                TokenType = AuthConstants.JwtTokenType,
                RefreshToken = refreshToken.RefreshToken
            });
        }

        private IDataContext CreateDC()
        {
            string cs = "default";
            return _configs.Connections.Where(x=>x.Key.ToLower() == cs).FirstOrDefault().CreateDC();
        }


        /// <summary>
        /// refresh token
        /// </summary>
        /// <param name="refreshToken">refreshToken</param>
        /// <returns></returns>
        public async Task<Token> RefreshTokenAsync(string refreshToken)
        {
            // 获取 RefreshToken
            PersistedGrant persistedGrant = await _dc.Set<PersistedGrant>().Where(x => x.RefreshToken == refreshToken).SingleOrDefaultAsync();
            if (persistedGrant != null)
            {
                // 校验 regresh token 有效期
                if (persistedGrant.Expiration < DateTime.Now)
                    throw new Exception("refresh token 已过期");

                // 删除 refresh token
                _dc.DeleteEntity(persistedGrant);
                await _dc.SaveChangesAsync();

                //生成并返回登录用户信息
                var loginUserInfo = new LoginUserInfo()
                {
                    Id = persistedGrant.UserId
                };

                // 清理过期 refreshtoken
                var sql = $"DELETE FROM {DC.GetTableName<PersistedGrant>()} WHERE Expiration<'{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}'";
                _dc.RunSQL(sql);
                _logger.LogDebug("清理过期的refreshToken：【sql:{0}】", sql);

                // 颁发 token
                return await IssueTokenAsync(loginUserInfo);
            }
            else
                throw new Exception("非法的 refresh Token");
        }

        /// <summary>
        /// clear expired refresh tokens
        /// </summary>
        /// <returns></returns>
        public async Task ClearExpiredRefreshTokenAsync()
        {
            var dataTime = DateTime.Now;
            var mapping = _dc.GetTableName<PersistedGrant>();
            var sql = $"DELETE FROM {mapping} WHERE Expiration<=@dataTime";
            _dc.RunSQL(sql, new
            {
                dataTime = dataTime
            });
            await Task.CompletedTask;
        }

    }
}
