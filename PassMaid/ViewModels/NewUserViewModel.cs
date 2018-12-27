﻿using Caliburn.Micro;
using PassMaid.Models;
using PassMaid.Utils;
using PassMaid.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PassMaid.ViewModels
{
    public class NewUserViewModel : Screen
    {
        private string _userUsername;
        private string _userPassword;
        private string _confirmUserPassword;

        public string UserUsername
        {
            get { return _userUsername; }
            set
            {
                _userUsername = value;
                NotifyOfPropertyChange(() => UserUsername);
            }
        }

        public string UserPassword
        {
            get { return _userPassword; }
            set
            {
                _userPassword = value;
                NotifyOfPropertyChange(() => UserPassword);
            }
        }
        
        public string ConfirmUserPassword
        {
            get { return _confirmUserPassword; }
            set
            {
                _confirmUserPassword = value;
                NotifyOfPropertyChange(() => ConfirmUserPassword);
            }
        }

        public ICommand CreateUserCommand => new RelayCommand(ExecuteCreateUser);

        public void ExecuteCreateUser(object o)
        {
            // Checks to see if both passwords match before creating a new user
            if (UserPassword == ConfirmUserPassword)
            {
                string hash = CryptoUtil.ComputeHash(UserPassword, HashType.SHA256, null);

                UserModel user = new UserModel
                {
                    Username = UserUsername,
                    Password = hash
                };

                // TODO: Store user credentials securely using hash/encryption
            }
        }

        public ICommand CancelCommand => new RelayCommand(ExecuteCancel);

        public void ExecuteCancel(object o)
        {
            // TODO: Go back to login view
        }
    }
}