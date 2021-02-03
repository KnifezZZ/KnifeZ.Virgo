using System;
using System.Collections.Generic;
using System.Linq;
using KnifeZ.Virgo.Core.ConfigOptions;

namespace KnifeZ.Virgo.Core
{
    /// <summary>
    /// Configs
    /// </summary>
    public class Configs
    {
        #region ConnectionStrings

        private List<CS> _connectStrings;

        /// <summary>
        /// ConnectionStrings
        /// </summary>
        public List<CS> Connections
        {
            get
            {
                if (_connectStrings == null)
                {
                    _connectStrings = new List<CS>();
                }
                return _connectStrings;
            }
            set
            {
                _connectStrings = value;
            }
        }

        #endregion

        #region Domains

        private Dictionary<string,FrameworkDomain> _domains;

        /// <summary>
        /// ConnectionStrings
        /// </summary>
        public Dictionary<string, FrameworkDomain> Domains
        {
            get
            {
                if (_domains == null)
                {
                    _domains = new Dictionary<string, FrameworkDomain>();
                }
                return _domains;
            }
            set
            {
                _domains = value;
                foreach (var item in _domains)
                {
                    if(item.Value != null)
                    {
                        item.Value.Name = item.Key;
                    }
                }
            }
        }

        #endregion


        #region QuickDebug

        private bool? _isQuickDebug;

        /// <summary>
        /// Is debug mode
        /// </summary>
        public bool IsQuickDebug
        {
            get
            {
                return _isQuickDebug ?? false;
            }
            set
            {
                _isQuickDebug = value;
            }
        }

        #endregion

        public string ErrorHandler { get; set; } = "/_Framework/Error";

        #region Cookie prefix

        private string _cookiePre;

        /// <summary>
        /// Cookie prefix
        /// </summary>
        public string CookiePre
        {
            get
            {
                return _cookiePre ?? string.Empty;
            }
            set
            {
                _cookiePre = value;
            }
        }

        #endregion

        #region Auto sync db

        private bool? _syncdb;

        /// <summary>
        /// Auto sync db(not supportted)
        /// </summary>
        public bool SyncDB
        {
            get
            {
                return _syncdb ?? false;
            }
            set
            {
                _syncdb = value;
            }
        }

        #endregion

        #region EncryptKey

        private string _encryptKey;

        /// <summary>
        /// EncryptKey
        /// </summary>
        public string EncryptKey
        {
            get
            {
                if (string.IsNullOrEmpty(_encryptKey))
                {
                    _encryptKey = string.Empty;
                }
                return _encryptKey;
            }
            set
            {
                _encryptKey = value;
            }
        }

        #endregion

        #region Custom settings

        private Dictionary<string,string> _appSettings;

        /// <summary>
        /// Custom settings
        /// </summary>
        public Dictionary<string, string> AppSettings
        {
            get
            {
                if (_appSettings == null)
                {
                    _appSettings = new Dictionary<string, string>();
                }
                return _appSettings;
            }
            set
            {
                _appSettings = value;
            }
        }

        #endregion

        #region Data Privilege

        private List<IDataPrivilege> _dataPrivilegeSettings;

        /// <summary>
        /// Data Privilege
        /// </summary>
        public List<IDataPrivilege> DataPrivilegeSettings
        {
            get
            {
                if (_dataPrivilegeSettings == null)
                {
                    _dataPrivilegeSettings = new List<IDataPrivilege>();
                }
                return _dataPrivilegeSettings;
            }
            set
            {
                _dataPrivilegeSettings = value;
            }
        }

        #endregion

        #region DFS Config

        private DFS _dfsServer;

        /// <summary>
        /// DFS Config
        /// </summary>
        public DFS DFSServer
        {
            get
            {
                if (_dfsServer == null)
                {
                    _dfsServer = new DFS();
                }
                return _dfsServer;
            }
            set
            {
                _dfsServer = value;
            }
        }

        #endregion

        #region FileOptions

        private FileUploadOptions _fileUploadOptions;

        /// <summary>
        /// FileOptions
        /// </summary>
        public FileUploadOptions FileUploadOptions
        {
            get
            {
                if (_fileUploadOptions == null)
                {
                    _fileUploadOptions = new FileUploadOptions()
                    {
                        UploadLimit = DefaultConfigConsts.DEFAULT_UPLOAD_LIMIT,
                        SaveFileMode = SaveFileModeEnum.Local,
                        Settings = new Dictionary<string, List<FileHandlerOptions>>()
                    };
                }
                return _fileUploadOptions;
            }
            set
            {
                _fileUploadOptions = value;
            }
        }

        #endregion

        #region UIOptions

        private UIOptions _uiOptions;

        /// <summary>
        /// UIOptions
        /// </summary>
        public UIOptions UiOptions
        {
            get
            {
                if (_uiOptions == null)
                {
                    _uiOptions = new UIOptions();
                    if (_uiOptions.PageSize == 0)
                    {
                        _uiOptions.PageSize = DefaultConfigConsts.DEFAULT_PAGESIZE;
                    }
                }
                return _uiOptions;
            }
            set
            {
                _uiOptions = value;
            }
        }

        #endregion

        #region Is FileAttachment public

        private bool? _isFilePublic;

        /// <summary>
        /// Is FileAttachment public
        /// </summary>
        public bool IsFilePublic
        {
            get
            {
                return _isFilePublic ?? false;
            }
            set
            {
                _isFilePublic = value;
            }
        }

        #endregion


        #region Cors configs

        private Cors _cors;

        /// <summary>
        ///  Cors configs
        /// </summary>
        public Cors CorsOptions
        {
            get
            {
                if (_cors == null)
                {
                    _cors = new Cors
                    {
                        Policy = new List<CorsPolicy>()
                    };
                }
                return _cors;
            }
            set
            {
                _cors = value;
            }
        }

        #endregion

        #region Support Languages

        private string _languages;

        /// <summary>
        /// Support Languages
        /// </summary>
        public string Languages
        {
            get
            {
                if (string.IsNullOrEmpty((_languages)))
                {
                    _languages = "zh";
                }
                return _languages;
            }
            set
            {
                _languages = value;
            }
        }

        #endregion


        public string HostRoot { get; set; } = "";


        #region CookieOption configs

        private CookieOption _cookieOption;

        /// <summary>
        ///  Cors configs
        /// </summary>
        public CookieOption CookieOption
        {
            get
            {
                if (_cookieOption == null)
                {
                    _cookieOption = new CookieOption();
                }
                return _cookieOption;
            }
            set
            {
                _cookieOption = value;
            }
        }

        #endregion

        #region JwtOption configs

        private JwtOption _jwtOption;

        /// <summary>
        ///  Cors configs
        /// </summary>
        public JwtOption JwtOption
        {
            get
            {
                if (_jwtOption == null)
                {
                    _jwtOption = new JwtOption();
                }
                return _jwtOption;
            }
            set
            {
                _jwtOption = value;
            }
        }

        #endregion

        public IDataContext CreateDC (string csName = null)
        {
            if (string.IsNullOrEmpty(csName))
            {
                csName = "default";
            }
            var cs = Connections.Where(x => x.Key.ToLower() == csName.ToLower()).SingleOrDefault();
            return cs?.CreateDC();
        }
    }
}
