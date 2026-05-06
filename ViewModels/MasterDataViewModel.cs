using Microsoft.EntityFrameworkCore;
using PrintOrderManager.Data;
using PrintOrderManager.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace PrintOrderManager.ViewModels
{
    public class MasterDataViewModel : INotifyPropertyChanged, IDisposable
    {
        private AppDbContext _db;

        public ObservableCollection<Material> Materials { get; set; }
        public ObservableCollection<Process> Processes { get; set; }

        public RelayCommand SaveCommand { get; set; }
        public RelayCommand DeleteMaterialCommand { get; set; }
        public RelayCommand DeleteProcessCommand { get; set; }

        public Action? CloseAction { get; set; }

        public MasterDataViewModel()
        {
            _db = new AppDbContext();
            _db.Materials.Load();
            _db.Processes.Load();

            Materials = _db.Materials.Local.ToObservableCollection();
            Processes = _db.Processes.Local.ToObservableCollection();

            SaveCommand = new RelayCommand(_ => Save());
            DeleteMaterialCommand = new RelayCommand(DeleteMaterial, CanDeleteMaterial);
            DeleteProcessCommand = new RelayCommand(DeleteProcess, CanDeleteProcess);
        }

        private bool CanDeleteMaterial(object? parameter) => parameter is Material;
        private void DeleteMaterial(object? parameter)
        {
            if (parameter is Material material)
            {
                var result = MessageBox.Show($"Bạn có chắc chắn muốn xóa chất liệu '{material.Name}'?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    Materials.Remove(material);
                }
            }
        }

        private bool CanDeleteProcess(object? parameter) => parameter is Process;
        private void DeleteProcess(object? parameter)
        {
            if (parameter is Process process)
            {
                var result = MessageBox.Show($"Bạn có chắc chắn muốn xóa gia công '{process.Name}'?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    Processes.Remove(process);
                }
            }
        }

        private void Save()
        {
            try
            {
                _db.SaveChanges();
                MessageBox.Show("Lưu thay đổi thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                CloseAction?.Invoke();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi lưu: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void Dispose()
        {
            _db.Dispose();
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
