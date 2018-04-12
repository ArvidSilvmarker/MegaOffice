using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MegaOffice
{
    class CategoryMenu
    {
        public Category EnterNewCategory()
        {
            return new Category();

        }

        private Category GetCategoryFromUser()
        {
            PrintCategories(_db.ReadAllCategories());
            WriteInColor("Välj kategori: ", ConsoleColor.DarkMagenta);
            return _db.ReadCategory(Convert.ToInt32(Console.ReadLine()) - 1);
        }

        private void PrintCategories(List<Category> categoryList)
        {
            WriteInColor($"{"Kategorinummer",-15}{"Namn",-22}", ConsoleColor.Red);
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
