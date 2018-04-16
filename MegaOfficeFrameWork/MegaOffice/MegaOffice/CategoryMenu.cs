using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MegaOffice
{
    public class CategoryMenu
    {
        private DatabaseManager _db;
        private MainMenu _mainMenu;
        public CategoryMenu(DatabaseManager db, MainMenu mainMenu)
        {
            this._db = db;
            this._mainMenu = mainMenu;
        }
        public Category EnterNewCategory()
        {
            return new Category(10, "Godis");
        }

        public Category GetCategoryFromUser()
        {
            PrintCategories(_db.ReadAllCategories());
            WriteInColor("Välj kategori: ", ConsoleColor.DarkMagenta);
            return _db.ReadCategory(Convert.ToInt32(Console.ReadLine()));
        }

        private void PrintCategories(List<Category> categoryList)
        {
            WriteLineInColor($"{"Kategorinummer",-15}{"Namn",-22}", ConsoleColor.Red);
            foreach (var category in categoryList)
            {
                Console.WriteLine($"{category.CategoryID,-15}{category.Name,-22}");
            }

            Console.WriteLine();
        }

        public void WriteInColor(string text, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.Write(text);
            Console.ResetColor();
        }

        public void WriteLineInColor(string text, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(text);
            Console.ResetColor();
        }
    }
}
