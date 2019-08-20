using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace WebDT.Models
{
    public class SignalModel
    {
        private WebMayTinhEntities db = new WebMayTinhEntities();
        public List<long> TaoKhoa()
        {
            ReRadom://Radom để chọn lại khóa
            //Radom để chọn khóa
            Random r = new Random();
            long x = r.Next(1001, 9997);
            long y = r.Next(1001, 9997);
            while (CHECK_SNT(x) == false || CHECK_SNT(y) == false)
            {
                Random rr = new Random();
                x = rr.Next(1001, 9997);
                y = rr.Next(1001, 9997);
            }


            List<long> Khoa = new List<long>(); // khai báo mảng chứa khóa 1,2 PriKey 3,4 PubKey

            long N = x * y; //Tính số hàm modulo của hệ thống 
            long phi = (x - 1) * (y - 1); //Tính giá trị hàm số Ơ-le
            long E = 0;
            for (long i = 17; i < phi; i++) //Tìm số nguyên tố cùng nhau của E vs phi trong khoảng từ 1 <= E <= phi
            {
                if (UOC_CHUNG_LON_NHAT(i, phi) == 1)
                {
                    E = i;

                    break;
                }
            }

            //Kiểm tra có lặp khóa công khai không.
            if (!CheckKeyPublic(N, E))
                goto ReRadom;

            //long k = nd(soE, phi);
            //Tính khóa giải mã D sao cho D*E = 1(mod phi(n))
            long k = 1;
            while (((phi * k + 1) % E != 0))
            { k++; }

            long soD = (phi * k + 1) / E; //tính số D

            Khoa.Add(soD);
            Khoa.Add(N);
            Khoa.Add(E);
            Khoa.Add(N);
            return Khoa;
        }

        //Kiểm tra khóa công khai có bị trùng k
        public bool CheckKeyPublic(long n, long e)
        {
            var model = db.AccountPayments.Where(x => x.so_n == n && x.so_e == e).ToList();
            if (model.Count > 0)
                return false;
            return true;
        }

        // tim uoc chung lon nhat
        public int UOC_CHUNG_LON_NHAT(long x, long y)
        {
            int uoc = 1;
            for (int i = 1; i <= x; i++)
            {
                if (x % i == 0 && y % i == 0) uoc = i;
            }
            return uoc;
        }

        // kiem tra so nguyen to
        public bool CHECK_SNT(long n)
        {
            int dem = 0;
            if (n < 2) return false;
            for (int i = 2; i <= n; i++)
            {
                if (n % i == 0) dem++;
            }
            if (dem == 1) return true;
            else return false;
        }

        string key = "A!9HHhi%XjjYY4YP2@Nob009X*1234567890!@#$%^&*()14344*";

        public string Encrypt_MD5(string text)
        {
            using (var md5 = new MD5CryptoServiceProvider())
            {
                using (var tdes = new TripleDESCryptoServiceProvider())
                {
                    tdes.Key = md5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
                    tdes.Mode = CipherMode.ECB;
                    tdes.Padding = PaddingMode.PKCS7;

                    using (var transform = tdes.CreateEncryptor())
                    {
                        byte[] textBytes = UTF8Encoding.UTF8.GetBytes(text);
                        byte[] bytes = transform.TransformFinalBlock(textBytes, 0, textBytes.Length);
                        return Convert.ToBase64String(bytes, 0, bytes.Length);
                    }
                }
            }
        }

        public string Decrypt_MD5(string cipher)
        {
            using (var md5 = new MD5CryptoServiceProvider())
            {
                using (var tdes = new TripleDESCryptoServiceProvider())
                {
                    tdes.Key = md5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
                    tdes.Mode = CipherMode.ECB;
                    tdes.Padding = PaddingMode.PKCS7;

                    using (var transform = tdes.CreateDecryptor())
                    {
                        byte[] cipherBytes = Convert.FromBase64String(cipher);
                        byte[] bytes = transform.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);
                        return UTF8Encoding.UTF8.GetString(bytes);
                    }
                }
            }
        }

        //hàm băm tin
        public byte[] hash(string xau)
        {
            byte[] textBytes = Encoding.Default.GetBytes(xau);
            try
            {
                MD5CryptoServiceProvider cryptHandler;
                cryptHandler = new MD5CryptoServiceProvider();
                byte[] hash = cryptHandler.ComputeHash(textBytes);

                return hash;
            }
            catch
            {
                throw;
            }
        }


        public long TINHA(long a, long b, long p)
        {
            long ret = 1;
            a %= p;
            b %= p - 1;
            while (b > 0) //vòng lặp phân tích b thành cơ số 2
            {
                if (b % 2 > 0)  //ở vị trí có số 1 thì nhân với a^(2^i) tương ứng. Tất cả các phép nhân đều có phép mod p theo sau.
                    ret = ret * a % p;
                a = a * a % p;  //tính tiếp a^(2^(i+1)), a^1 -> a^2 -> a^4 -> a^8 -> a^16 v.v...
                b /= 2;
            }
            return (long)ret;
        }
    }
}