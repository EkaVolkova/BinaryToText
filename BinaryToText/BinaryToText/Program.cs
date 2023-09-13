using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

/// <summary>
/// Написать программу-загрузчик данных из бинарного формата в текст.
/// На вход программа получает бинарный файл, предположительно, это база данных студентов.
/// Свойства сущности Student:
///     Имя — Name(string);
///     Группа — Group(string);
///     Дата рождения — DateOfBirth (DateTime).
/// Ваша программа должна:
/// Создать на рабочем столе директорию Students.
/// Внутри раскидать всех студентов из файла по группам (каждая группа-отдельный текстовый файл), 
/// в файле группы студенты перечислены построчно в формате "Имя, дата рождения".
/// Пространство имен namespace должно быть, как FinalTask.
/// </summary>
namespace FinalTask
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                //Считываем студентов из файла
                var students = Student.GetStudents(args[0]);
                //Записываем студентов в файл
                Student.WriteStudents(students, args[1]);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }

    /// <summary>
    /// Класс для описания студентов
    /// </summary>
    [Serializable]
    class Student
    {
        public string Name { get; set; }
        public string Group { get; set; }
        public DateTime Birthday { get; set; }

        /// <summary>
        /// Статический класс для формирования массива студентов из файла
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static Student[] GetStudents(string path)
        {
            //Проверка, что путь задан
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException("Не задан файл");

            //Создали экземпляр класса для работы с файлом
            FileInfo fileInfo = new FileInfo(path);

            //ПРоверили, что файл существует
            if (!fileInfo.Exists)
                throw new ArgumentException("Файл не существует");

            BinaryFormatter formatter = new BinaryFormatter();

            //Создали массив студентов
            Student[] deserilizeStudents;

            using (var fs = new FileStream(path, FileMode.OpenOrCreate))
            {
                //Десериализовали студентов из файла
                deserilizeStudents = (Student[])formatter.Deserialize(fs);

            }

            return deserilizeStudents;
        }
        public static void WriteStudents(Student[] students, string path)
        {
            //Создали директорию, в которую будем заносить данные о студентах
            DirectoryInfo directoryInfo = new DirectoryInfo(path);

            if (!directoryInfo.Exists)
                directoryInfo.Create();

            //Создали список групп
            List<string> groups = new List<string>();

            //Прошлись по всему массиву студентов, записали все уникальные группы
            foreach (var student in students)
            {
                if (!groups.Contains(student.Group))
                    groups.Add(student.Group);

            }

            BinaryFormatter formatter = new BinaryFormatter();

            foreach (var group in groups)
            {
                //Для каждой группы создали свой файл и если студент принадлежит группе, записали его в список
                using (var fs = new StreamWriter(path + @"\" + group + ".txt"))
                {
                    foreach (var student in students)
                    {
                        if (student.Group == group)
                            fs.WriteLine(student.Name + ", " + student.Birthday.ToString("d"));
                    }
                }
            }
        }


    }

}
