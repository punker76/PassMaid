﻿using Caliburn.Micro;
using MahApps.Metro.Controls.Dialogs;
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
    public class EditPasswordViewModel : PasswordScreen
    {
        public IDialogCoordinator dialogCoordinator;

        public GeneratorDialogView GeneratorDialog { get; set; }

        public EditPasswordViewModel(PasswordModel _selectedPassword, VaultViewModel _vaultViewModel) : base(_selectedPassword, _vaultViewModel)
        {
            if (SelectedPassword != null)
            {
                this.Name = SelectedPassword.Name;
                this.Website = SelectedPassword.Website;
                this.Username = SelectedPassword.Username;
                this.Password = SelectedPassword.Password;
            }

            dialogCoordinator = DialogCoordinator.Instance;
        }

        public ICommand ToggleVisibilityCommand => new RelayCommand(ExecuteToggleVisibility);

        public void ExecuteToggleVisibility(object o)
        {
            // TODO: Implement password mask (using PasswordBox/SecureString)
        }

        public ICommand GeneratePasswordCommand => new RelayCommand(ExecuteGeneratePassword);

        public async void ExecuteGeneratePassword(object o)
        {
            var dialogViewModel = new GeneratorDialogViewModel(this);
            GeneratorDialog = new GeneratorDialogView()
            {
                DataContext = dialogViewModel
            };

            await dialogCoordinator.ShowMetroDialogAsync(this, GeneratorDialog);
        }

        public ICommand SaveCommand => new RelayCommand(ExecuteSave);

        public void ExecuteSave(object o)
        {
            VaultVM.SelectedPasswordModel.Name = this.Name;
            VaultVM.SelectedPasswordModel.Website = this.Website;
            VaultVM.SelectedPasswordModel.Username = this.Username;

            // Encrypts and stores the password in the database
            byte[] passwordBytes = ASCIIEncoding.ASCII.GetBytes(this.Password);
            byte[] masterKey = CryptoUtil.MasterKey;
            byte[] encryptedPassword = CryptoUtil.AES_GCMEncrypt(passwordBytes, masterKey);

            VaultVM.SelectedPasswordModel.Password = Convert.ToBase64String(encryptedPassword);
            SQLiteDataAccess.UpdatePassword(VaultVM.SelectedPasswordModel);

            // Updates password in bindable collection
            VaultVM.SelectedPasswordModel.Password = this.Password;
            VaultVM.PassScreenType = new DisplayPasswordViewModel(SelectedPassword, VaultVM);
        }

        public ICommand CancelCommand => new RelayCommand(ExecuteCancel);

        public void ExecuteCancel(object o)
        {
            VaultVM.PassScreenType = new DisplayPasswordViewModel(SelectedPassword, VaultVM);
        }

        public ICommand DeleteCommand => new RelayCommand(ExecuteDelete);

        public void ExecuteDelete(object o)
        {
            SQLiteDataAccess.DeletePassword(VaultVM.SelectedPasswordModel);
            VaultVM.Passwords.Remove(VaultVM.SelectedPasswordModel);

            VaultVM.PassScreenType = new DisplayPasswordViewModel(null, VaultVM);
        }
    }
}
