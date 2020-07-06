using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Docm.Business.Guest.Imp
{
    public interface IGuest
    {
        Task<string> Logon(string account, string password);
    }
    [Serializable]
    public class AccountInfo
    {
        /// <summary>
        /// 账户
        /// </summary>
        public string account { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        public string password { get; set; }
    }
}
