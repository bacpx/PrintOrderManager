using System.IO;

namespace PrintOrderManager.Helpers
{
    public static class DatabaseHelper
    {
        public static void BackupDatabase()
        {
            var sourceFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data.db");
            var backupDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Backups");
            
            if (!Directory.Exists(backupDir))
            {
                Directory.CreateDirectory(backupDir);
            }

            if (File.Exists(sourceFile))
            {
                var destFile = Path.Combine(backupDir, $"data_backup_{DateTime.Now:yyyyMMdd_HHmmss}.db");
                File.Copy(sourceFile, destFile, true);
                System.Windows.MessageBox.Show($"Sao lưu dữ liệu thành công!\nFile: {destFile}", "Thông báo", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
            }
            else
            {
                System.Windows.MessageBox.Show("Không tìm thấy file cơ sở dữ liệu để sao lưu.", "Thông báo", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
            }
        }
    }
}
