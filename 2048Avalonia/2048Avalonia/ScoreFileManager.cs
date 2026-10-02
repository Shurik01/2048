using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace _2048Avalonia
{
    public static class ScoreFileManager
    {
        private static readonly string FolderPath = Path.Combine(
       Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
       "2048Avalonia"
        );

        private static readonly string FilePath = Path.Combine(FolderPath, "bestscore.txt");

        // Метод для сохранения
        public static void SaveBestScore(int score)
        {
            try
            {
                // Создаем папку, если её ещё нет
                Directory.CreateDirectory(FolderPath);
                // Записываем число в файл
                File.WriteAllText(FilePath, score.ToString());
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка сохранения: {ex.Message}");
            }
        }

        public static int LoadBestScore()
        {
            try
            {
                if (File.Exists(FilePath))
                {
                    string content = File.ReadAllText(FilePath);
                    if (int.TryParse(content, out int score))
                    {
                        return score;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка загрузки: {ex.Message}");
            }

            return 0; // Возвращаем 0, если файла нет или произошла ошибка
        }
    }
}
