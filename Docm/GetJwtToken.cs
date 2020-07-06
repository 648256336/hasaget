using CoreUtility;
using Docm.Business.Guest.Imp;
using MicroCommon.Model;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Docm
{
    public class GetJwtToken
    {
        private readonly TokenManagement _options;
        public GetJwtToken(IOptionsMonitor<TokenManagement> options)
        {
            _options = options.CurrentValue;
        }

        public string GetHssJwtToken(AccountInfo accountInfo)
        {
            //具体数据信息
            var claims = new[]
            {
                new Claim( ClaimTypes.Name,accountInfo.account)
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Secret));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var jwtToken = new JwtSecurityToken(_options.Issuer, _options.Audience, claims,
                expires: DateTime.Now.AddMinutes(_options.AccessExpiration),
                signingCredentials: credentials);
            return new JwtSecurityTokenHandler().WriteToken(jwtToken);
        }
        public string GetRsaJwtToken(AccountInfo accountInfo)
        {
            //具体数据信息
            var claims = new[]
            {
                new Claim( ClaimTypes.Name,accountInfo.account)
            };
            var path = Directory.GetCurrentDirectory();
            if (CryptoUtils.TryGetKeyParameters(path, true, out RSAParameters keyParams) == false)
            {
                keyParams = CryptoUtils.GenerateAndSaveKey(path);
            }
            
            var credentials = new SigningCredentials(new RsaSecurityKey(keyParams), SecurityAlgorithms.RsaSha256Signature);
            var jwtToken = new JwtSecurityToken(
                _options.Issuer, 
                _options.Audience,
                claims,
                expires: DateTime.Now.AddMinutes(_options.AccessExpiration),
                signingCredentials: credentials);
            return new JwtSecurityTokenHandler().WriteToken(jwtToken);
        }

        public string GetHss(AccountInfo accountInfo)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name,accountInfo.account),
                new Claim(ClaimTypes.Role,"Admin"),
                new Claim("password",accountInfo.password)
            };
            var credentials= new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Secret)),SecurityAlgorithms.HmacSha256);
            var JwtToken = new JwtSecurityToken(
                issuer: _options.Issuer,
                audience:_options.Audience,
                claims:claims,
                notBefore: DateTime.Now.AddMinutes(1),
                expires:DateTime.Now.AddMinutes(15),
                signingCredentials:credentials);
            return new JwtSecurityTokenHandler().WriteToken(JwtToken);
        }
    }
}
