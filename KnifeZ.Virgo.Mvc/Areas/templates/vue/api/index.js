import request from '@/utils/request'
import config from '@/configs/index'
import contentType from '@/configs/content-type'
const reqPath = config.headerApi + '/$name$/'
import {
	bSearch,
	bBatchDelete,
	bAdd,
	bEdit,
	bDetail,
	bExportExcel,
	bExportExcelByIds,
	bGetExcelTemplate,
	bImported,
} from '@/api/baseCURD'
const API = {
	// 列表查询
	Search(data) {
		return bSearch(reqPath, data)
	},
	// 批量删除
	BatchDelete(data) {
		return bBatchDelete(reqPath, data)
	},
	// 新增
	Add(data) {
		return bAdd(reqPath, data)
	},
	// 修改
	Edit(data) {
		return bEdit(reqPath, data)
	},
	// 详情
	Detail(id) {
		return bDetail(reqPath, id)
	},
	//导出excel
	ExportExcel(data) {
		return bExportExcel(reqPath,data)
	},
	ExportExcelByIds(data) {
		return bExportExcelByIds(reqPath, data)
	},
	GetExcelTemplate(data) {
		return bGetExcelTemplate(reqPath, data)
	},
	//导入
	Imported(data) {
		return bImported(reqPath, data)
	},
	$extAPIs$
}
export default API
