using System;

namespace ZM.Core.ApiItems
{
    public class ApiResult
    {
        public ResultCodeEnum resultCode { get; set; }
        public object resultBody { get; set; }
    }
    public enum ResultCodeEnum
    {
        OK = 0,
        InvalidParameter = -1,
        Bad = -10,
        InvalidAuth = -60,
        InvalidAuthPath = -61,
        InvalidAuthToken = -61,
        InvalidAuthAction = -62,
        InvalidIP = -70,
        InvalidAction = -80,
        InvalidTableRight = -81,
        InvalidEntityDatapPermission = -100,
        InvalidEntityDatapPermissionWriteAdd = -101,
        InvalidEntityDatapPermissionWriteUpdate = -102,
        InvalidEntityDatapPermissionWrite = -103,
        InvalidEntityDatapPermissionDelete = -104,
        Exception = -999
    }
}
