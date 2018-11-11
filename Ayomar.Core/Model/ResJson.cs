namespace Ayomar.Core.Model
{
    public class ResJson
    {
        /// <summary>
        /// 是否成功
        /// </summary>
        public bool success { get; set; } = true;
        /// <summary>
        /// 信息
        /// </summary>
        public string message { get; set; }

        /// <summary>
        /// 状态码
        /// </summary>
        public int statusCode { get; set; } = 200;
        /// <summary>
        /// 数据集
        /// </summary>
        public object data { get; set; }
        /// <summary>
        /// 审核信息
        /// </summary>
        public object aduitData { get; set; }
        /// <summary>
        /// 跳转链接
        /// </summary>
        public string returnUrl { get; set; }
    }
}
