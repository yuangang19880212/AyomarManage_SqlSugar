using System.Collections.Generic;

namespace Ayomar.Common.ResultHelper
{
    

    #region Layui
    //数据表格
    public class ResTable
    {
        /// <summary>
        /// 状态码
        /// </summary>
        public int status { get; set; } = 200;
        /// <summary>
        /// 记录数
        /// </summary>
        public int total { get; set; }
        /// <summary>
        /// 数据集
        /// </summary>
        public object data { get; set; }
    }
    #endregion

    #region 图像审核返回Model
    public class ImageCensoring
    {
        /// <summary>
        /// 请求唯一id
        /// </summary>
        public long log_id { get; set; }
        /// <summary>
        /// 错误提示信息，失败才返回，成功不返回
        /// </summary>
        public string error_msg { get; set; }
        /// <summary>
        /// 审核结果，成功才返回，失败不返回。可取值1.合规,2.疑似，3.不合规
        /// </summary>
        public string conclusion { get; set; }
        /// <summary>
        /// 审核项详细信息，响应成功并且conclusion为疑似或不合规时才返回，响应失败或conclusion为合规是不返回
        /// </summary>
        public List<ImageCensoringData> data { get; set; }
    }

    public class ImageCensoringData
    {
        /// <summary>
        /// 不合规项描述信息
        /// </summary>
        public string msg { get; set; }
        /// <summary>
        /// 不合规项置信度
        /// </summary>
        public double probability { get; set; }
        /// <summary>
        /// 审核类型，1：色情、2：性感、3：暴恐、4:恶心、5：水印码、6：二维码、7：条形码、8：政治人物、9：敏感词、10：自定义敏感词
        /// </summary>
        public int type { get; set; }
        /// <summary>
        /// 审核不通过敏感词，只有敏感词审核不通过才有
        /// </summary>
        public string words { get; set; }

        public List<ImageCensoringDataStars> stars { get; set; }
    }
    public class ImageCensoringDataStars
    {
        /// <summary>
        /// 不合规项置信度
        /// </summary>
        public double probability { get; set; }
        /// <summary>
        /// 政治人物敏感名
        /// </summary>
        public string name { get; set; }
    }
    #endregion
}
