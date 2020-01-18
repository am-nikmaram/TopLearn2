using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TopLearn.Core.Convertors;
using TopLearn.Core.DTOs;
using TopLearn.Core.DTOs.User;
using TopLearn.Core.Generator;
using TopLearn.Core.Security;
using TopLearn.Core.Services.Interfaces;
using TopLearn.DataLayer.Context;
using TopLearn.DataLayer.Entities.User;
using TopLearn.DataLayer.Entities.Wallet;

namespace TopLearn.Core.Services
{
    public class UserService : IUserService
    {
        TopLearnContext _context;
        public UserService(TopLearnContext context)
        {
            _context = context;
        }

        public bool ActiveAccount(string activeCode)
        {
            var user = _context.Users.SingleOrDefault(u => u.ActiveCode == activeCode);
            if (user == null || user.IsActive)
                return false;
            user.IsActive = true;
            user.ActiveCode = NameGenerator.GenerateUniqCode();
            _context.SaveChanges();
            return true;
        }

        public int adduser(User user)
        {
            _context.Users.Add(user);
            _context.SaveChanges();
            return user.UserId;
        }

        public int AddUserFromAdmin(CreateUserViewModel user)
        {
            User User = new User();
            User.Password = PasswordHelper.EncodePasswordMd5(user.Password);
            User.ActiveCode = NameGenerator.GenerateUniqCode();
            User.Email = user.Email;
            User.IsActive = true;
            User.RegisterDate = DateTime.Now;
            User.UserName = user.UserName;

            #region Save Avatar
            if (user.UserAvatar != null)
            {
                string imgpath = "";

                User.UserAvatar = NameGenerator.GenerateUniqCode() + Path.GetExtension(user.UserAvatar.FileName);
                string test = Directory.GetCurrentDirectory().ToString() + "/wwwroot/UserAvatar";
                imgpath = Path.Combine(test, User.UserAvatar);
                using (var stream = new FileStream(imgpath, FileMode.Create))
                {
                    user.UserAvatar.CopyTo(stream);
                }
            }
            #endregion

            return adduser(User);


        }

        public int AddWallet(Wallet wallet)
        {
            _context.Wallets.Add(wallet);
            _context.SaveChanges();
            return wallet.WalletId;
        }

        public int BalanceUserWallet(string userName)
        {
            int userId = GetUserIdByUserName(userName);
            var enter = _context.Wallets.Where(w => w.UserId == userId && w.TypeId == 1&&w.IsPay).Select(w => w.Amount).ToList();
            var exit= _context.Wallets.Where(w => w.UserId == userId && w.TypeId == 2).Select(w => w.Amount).ToList();
            return (enter.Sum() - exit.Sum());

        }

        public void ChangeUserPassword(string userName, string newPassword)
        {
            var user = GetUserByUserName(userName);
            user.Password = PasswordHelper.EncodePasswordMd5(newPassword);
            this.UpdateUser(user);

        }

        public int ChargeWallet(string userName, int Amount, string description, bool isPay = false)
        {
            Wallet wallet = new Wallet()
            {
                Amount = Amount,
                CreateDate = DateTime.Now,
                Description = description,
                IsPay = isPay,
                TypeId = 1,
                UserId = GetUserIdByUserName(userName)
            };
           return AddWallet(wallet);
        }

        public bool CompareOldPassword(string oldPassword, string username)
        {
            string hashOldPassword = PasswordHelper.EncodePasswordMd5(oldPassword);
            return _context.Users.Any(u => u.UserName == username && u.Password == hashOldPassword);
        }

        public void DeleteUser(int userId)
        {
            User user = GetUserById(userId);
            user.IsDelete = true;
            UpdateUser(user);
        }

        public void EditProfile(string username,EditProfileViewModel profile)
        {
            if (profile.UserAvatar != null)
            {
                string imgpath = "";
                if (profile.AvatarName != "Defult.jpg")
                {
                    string oldfile = Directory.GetCurrentDirectory().ToString() + "/wwwroot/UserAvatar";
                    imgpath = Path.Combine(oldfile, profile.AvatarName);
                    if (File.Exists(imgpath))
                    {
                        File.Delete(imgpath);
                    }
                }
                profile.AvatarName = NameGenerator.GenerateUniqCode() + Path.GetExtension(profile.UserAvatar.FileName);
                string test = Directory.GetCurrentDirectory().ToString() + "/wwwroot/UserAvatar";
                imgpath = Path.Combine(test, profile.AvatarName);
                using (var stream = new FileStream(imgpath, FileMode.Create))
                {
                    profile.UserAvatar.CopyTo(stream);
                }
            }
                var user = GetUserByUserName(username);
                user.Email = profile.Email;
                user.UserName = profile.UserName;
                user.UserAvatar = profile.AvatarName;
                UpdateUser(user);
                //_context.Update(user);
            
        }

        public void EditUserFromAdmin(EditUserViewModel editUser)
        {
            User user = GetUserById(editUser.UserId);
            user.Email = editUser.Email;
            if(!string.IsNullOrEmpty(editUser.Password))
            {
                user.Password = PasswordHelper.EncodePasswordMd5(editUser.Password);
            }
            if(editUser.UserAvatar!=null)
            {
                if(editUser.AvatarName!= "Defult.jpg")
                {
                    //TODO Delete previous pic
                    string deletePath = Directory.GetCurrentDirectory().ToString() + "/wwwroot/UserAvatar";
                    deletePath = Path.Combine(deletePath, editUser.AvatarName);
                    if (File.Exists(deletePath))
                        File.Delete(deletePath);
                }
                
                //SAVE New pic
                user.UserAvatar = NameGenerator.GenerateUniqCode() + Path.GetExtension(editUser.UserAvatar.FileName);
                string imgpath = Directory.GetCurrentDirectory().ToString() + "/wwwroot/UserAvatar";
                imgpath = Path.Combine(imgpath, user.UserAvatar);
                using (var stream = new FileStream(imgpath, FileMode.Create))
                {
                    editUser.UserAvatar.CopyTo(stream);
                }
            }
            _context.Users.Update(user);
            _context.SaveChanges(); 

        }
       // private SaveImage()

        public EditProfileViewModel GetDataForEditProfileUser(string username)
        {
            return _context.Users.Where(u => u.UserName == username).Select(u => new EditProfileViewModel()
            {
                AvatarName = u.UserAvatar,
                Email = u.Email,
                UserName=u.UserName
            }).Single();
        }

        public UserForAdminViewModel GetDeleteUsers(int pageId = 1, string filterEmail = "", string filterUserName = "")
        {
            IQueryable<User> result = _context.Users.IgnoreQueryFilters().Where(u=>u.IsDelete);
            if (!string.IsNullOrEmpty(filterEmail))
            {
                result = result.Where(u => u.Email.Contains(filterEmail));
            }
            if (!string.IsNullOrEmpty(filterUserName))
            {
                result = result.Where(u => u.UserName.Contains(filterUserName));
            }
            int take = 20;
            int skip = (pageId - 1) * take;
            UserForAdminViewModel List = new UserForAdminViewModel();
            List.CurrentPage = pageId;
            List.PageCount = result.Count() / take;
            List.Users = result.OrderBy(u => u.RegisterDate).Skip(skip).Take(take).ToList();

            return List;
        }

        public SideBarUserPanelViewModel GetSideBarUserPanelData(string username)
        {
            return _context.Users.Where(u => u.UserName == username).Select(u => new SideBarUserPanelViewModel()
            {
                UserName = u.UserName,
                ImageName = u.UserAvatar,
                RegisterDate = u.RegisterDate
                
            }).Single();
        }

        public User GetUserByActiveCode(string activeCode)
        {
            return _context.Users.SingleOrDefault(u => u.ActiveCode == activeCode);
        }

        public User GetUserByEmail(string email)
        {
            return _context.Users.SingleOrDefault(u => u.Email == email);
        }

        public User GetUserById(int userId)
        {
            return _context.Users.Find(userId);
        }

        public User GetUserByUserName(string username)
        {
            return _context.Users.SingleOrDefault(u => u.UserName == username);
        }

        public EditUserViewModel GetUserForShowInEditMode(int userId)
        {
            return _context.Users.Where(u => u.UserId == userId)
                .Select(u => new EditUserViewModel()
                {
                    UserId = u.UserId,
                     UserName=u.UserName,
                      Email=u.Email,
                       AvatarName=u.UserAvatar,
                        UserRoles=u.UserRoles.Select(r=>r.RoleId).ToList()
                }).Single();
        }

        public int GetUserIdByUserName(string userName)
        {
            return _context.Users.Single(u => u.UserName == userName).UserId;
        }

        public InformationUserViewModel GetUserInformation(string username)
        {
            var user = GetUserByUserName(username);
            InformationUserViewModel information = new InformationUserViewModel();
            information.UserName = user.UserName; 
            information.Email = user.Email;
            information.RegisterDate = user.RegisterDate;
            information.Wallet = BalanceUserWallet(username);

            return information;
        }

        public InformationUserViewModel GetUserInformation(int userId)
        {
            var user = GetUserById(userId);
            InformationUserViewModel information = new InformationUserViewModel();
            information.UserName = user.UserName;
            information.Email = user.Email;
            information.RegisterDate = user.RegisterDate;
            information.Wallet = BalanceUserWallet(user.UserName);

            return information;
        }

        public UserForAdminViewModel GetUsers(int pageId = 1, string filterEmail = "", string filterUserName = "")
        {
            IQueryable<User> result = _context.Users;
            if(!string.IsNullOrEmpty(filterEmail))
            {
                result = result.Where(u => u.Email.Contains(filterEmail));
            }
            if(!string.IsNullOrEmpty(filterUserName))
            {
                result = result.Where(u => u.UserName.Contains(filterUserName));
            }
            int take = 20;
            int skip = (pageId-1)*take;
            UserForAdminViewModel List = new UserForAdminViewModel();
            List.CurrentPage = pageId;
            List.PageCount = result.Count() / take;
            List.Users = result.OrderBy(u => u.RegisterDate).Skip(skip).Take(take).ToList();
            
            return List;
        }

        public Wallet GetWalletByWalletId(int walletId)
        {
            return _context.Wallets.Find(walletId);
        }

        public List<WalletViewModel> GetWalletUser(string userName)
        {
            int userId = GetUserIdByUserName(userName);
            return _context.Wallets.Where(w => w.IsPay && w.UserId == userId).Select(w => new WalletViewModel()
            {
                Amount = w.Amount,
                DateTime = w.CreateDate,
                Description = w.Description,
                Type = w.TypeId
            }).ToList();
        }

        public bool IsExistEmail(string email)
        {
            return _context.Users.Any(u => u.Email == email);
        }

        public bool IsExistUserName(string userName)
        {
            return _context.Users.Any(u => u.UserName == userName);
        }

        public User LoginUser(LoginViewModel login)
        {
            string hashPassword = PasswordHelper.EncodePasswordMd5(login.Password);
            string email = FixedText.FixEmail(login.Email);
            return _context.Users.SingleOrDefault(u => u.Email == email && u.Password == hashPassword);

        }

        public void UpdateUser(User user)
        {
            _context.Update(user);
            _context.SaveChanges();
        }

        public void UpdateWallet(Wallet wallet)
        {
            _context.Wallets.Update(wallet);
            _context.SaveChanges();
        }
    }
}
