using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using System.Windows.Forms;

namespace Roll20Bot.Modules
{
    public class Commands : ModuleBase<SocketCommandContext>
    {
        Random random = new Random();

        private Color Clr = Color.Gold;
        private string txt;
        private string finalStr;
        private string[] txtPack = new string[10] {
            "Отличный бросок!",
            "Можно и лучше.",
            "Такова судьба.",
            "Сегодня определенно твой день!",
            "Хм.. Сойдёт.",
            "У тебя было множество вариантов успеха и только один провальный, и угадай какой у тебя выпал?",
            "Возле тебя случайно черная кошка мимо не пробегала?",
            "Таков путь.",
            "Как в казино.",
            "По моему кто-то подкручиват броски."
        };



        [Command("roll")]
        [Alias("Roll", "R", "r", "dice", "Dice", "D", "d")]
        public async Task roll(string InputStr)
        {
            Embed embed;
            var quantityXfacets = HelpRoll(InputStr);

            if (quantityXfacets.Key < 1 || quantityXfacets.Value < 1)
            {
                var EmbedBuilder = new EmbedBuilder()
                .WithColor(Clr)
                .WithTitle("Неправильный формат запроса.")
                .WithFooter(footer =>
                {
                    footer
                    .WithText("Воспользуйтесь `!help`");
                });
                embed = EmbedBuilder.Build();
            }
            else
            {
                int r = 0;
                for (int i = 0; i < quantityXfacets.Key; i++)
                {
                    int t = random.Next(1, quantityXfacets.Value + 1);
                    r += t;
                    finalStr += t.ToString();
                    if (i != quantityXfacets.Key - 1)
                    {
                        finalStr += " + ";
                    }

                }


                if (r == 1)
                {
                    Clr = Color.Red;
                    txt = "КРИТИЧЕСКИЙ ПРОВАЛ";
                }
                else if (r == quantityXfacets.Value * quantityXfacets.Key)
                {
                    Clr = Color.Blue;
                    txt = "КРИТИЧЕСКИЙ УСПЕХ";
                }
                else
                {
                    Clr = Color.Green;
                    txt = txtPack[random.Next(0, txtPack.Length)];
                }


                var EmbedBuilder = new EmbedBuilder()
                    .WithColor(Clr)
                    .WithTitle("Бросок - " + finalStr + " = " + r + "!")
                    .WithFooter(footer =>
                        {
                            footer
                            .WithText(txt);
                        });
                embed = EmbedBuilder.Build();
            }

            await ReplyAsync(embed: embed);
        }

        private KeyValuePair<int, int> HelpRoll(string inputStr)
        {
            int q = -1, f = -1;

            inputStr = inputStr.ToLower();
            if (inputStr.Contains('d'))
            {
                q = Convert.ToInt32(inputStr.Split('d')[0]);
                f = Convert.ToInt32(inputStr.Split('d')[1]);
                if (q < 1)
                    q *= -1;
                if (f < 1)
                    f *= -1;
            }
            if (inputStr.Contains('к'))
            {
                q = Convert.ToInt32(inputStr.Split('к')[0]);
                f = Convert.ToInt32(inputStr.Split('к')[1]);
                if (q < 1)
                    q *= -1;
                if (f < 1)
                    f *= -1;
            }

            return new KeyValuePair<int, int>(q, f);
        }

        [Command("help")]
        [Alias("Help", "h", "H", "Помощь", "П", "помощь", "п")]
        public async Task Help()
        {
            Embed embed;
            var EmbedBuilder = new EmbedBuilder()
                   .WithColor(Color.Teal)
                   .WithTitle("Команды")
                   .WithFooter(footer =>
                   {
                       footer
                        .WithText("Список команд бота:\n" +
                        "!h \n!help, !помощь\n\n" +
                        "Бросок кубика:\n" +
                        "!r XdY — x - число кубов, а y - кол-во граней \n(!roll, !dice)\n\n" +
                        "Узнать характеристики брони:\n" +
                        "!a Название \n!armor, !доспех\n\n" +
                        "Узнать характеристики оружия:\n" +
                        "!w Название \n!weapon, !оружие\n\n" +
                        "Узнать содержимое и описание инструментов\n" +
                        "!t Название\n!tool, !инструмент\n\n" +
                        "Узнать о конкретном снаряжении (стоимость, вес и др.):\n" +
                        "!e Название\n!equipment, !снаряжение\n\n" +
                        "Узнать описание любого заклинания:\n" +
                        "!m Название (если в заклинании 2 и больше слов, то слова нужно заключить в кавычки) \n!magic, !spell, !магия");
                   });
            embed = EmbedBuilder.Build();
            await ReplyAsync(embed: embed);
        }

        [Command("Magic")]
        [Alias("magic", "m", "M", "магия", "М", "Spell", "spell")]
        public async Task Spell(string InputStr)
        {
            
            string OutputStr = null;
            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(Application.StartupPath + @"\DB\spellsDnD.xml");

            XmlElement xRoot = xDoc.DocumentElement;
            foreach (XmlNode xnode in xRoot)
            {
                if (xnode.Attributes.Item(0).Value.ToString().ToUpper() == InputStr.ToUpper())
                {
                    foreach (XmlNode childnode in xnode.ChildNodes)
                    {
                        OutputStr += $"{childnode.InnerText}" + "\n";
                    }
                        
                }
            }

            if (OutputStr == null)
            {
                OutputStr = "Ничего не найдено. Попробуйте изменить запрос";
            }

                Embed embed;
            var EmbedBuilder = new EmbedBuilder()
                   .WithColor(Color.Teal)
                   .WithFooter(footer =>
                   {
                       footer
                        .WithText(OutputStr);
                   });
            embed = EmbedBuilder.Build();

            await ReplyAsync(embed: embed);
        }

        [Command("Armor")]
        [Alias("armor", "a", "Д", "Доспех", "д", "доспех")]
        public async Task Armor(string InputStr)
        {

            string OutputStr = null;
            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(Application.StartupPath + @"\DB\armorsDnD.xml");

            XmlElement xRoot = xDoc.DocumentElement;
            foreach (XmlNode xnode in xRoot)
            {
                if (xnode.Attributes.Item(0).Value.ToString().ToUpper() == InputStr.ToUpper())
                {
                    foreach (XmlNode childnode in xnode.ChildNodes)
                    {
                        OutputStr += $"{childnode.InnerText}" + "\n";
                    }

                }
            }

            if (OutputStr == null)
            {
                OutputStr = "Ничего не найдено. Попробуйте изменить запрос";
            }

            Embed embed;
            var EmbedBuilder = new EmbedBuilder()
                   .WithColor(Color.Teal)
                   .WithFooter(footer =>
                   {
                       footer
                        .WithText(OutputStr);
                   });
            embed = EmbedBuilder.Build();

            await ReplyAsync(embed: embed);
        }

        [Command("Weapon")]
        [Alias("weapon", "w", "О", "Оружие", "оружие", "о")]
        public async Task Weapon(string InputStr)
        {

            string OutputStr = null;
            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(Application.StartupPath + @"\DB\WeaponsDnD.xml");

            XmlElement xRoot = xDoc.DocumentElement;
            foreach (XmlNode xnode in xRoot)
            {
                if (xnode.Attributes.Item(0).Value.ToString().ToUpper() == InputStr.ToUpper())
                {
                    foreach (XmlNode childnode in xnode.ChildNodes)
                    {
                        OutputStr += $"{childnode.InnerText}" + "\n";
                    }

                }
            }

            if (OutputStr == null)
            {
                OutputStr = "Ничего не найдено. Попробуйте изменить запрос";
            }

            Embed embed;
            var EmbedBuilder = new EmbedBuilder()
                   .WithColor(Color.Teal)
                   .WithFooter(footer =>
                   {
                       footer
                        .WithText(OutputStr);
                   });
            embed = EmbedBuilder.Build();

            await ReplyAsync(embed: embed);
        }

        [Command("Equipment")]
        [Alias("equipment", "e","E", "С", "с", "Снаряжение", "снаряжение")]
        public async Task Equipment(string InputStr)
        {

            string OutputStr = null;
            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(Application.StartupPath + @"\DB\Equipment.xml");

            XmlElement xRoot = xDoc.DocumentElement;
            foreach (XmlNode xnode in xRoot)
            {
                if (xnode.Attributes.Item(0).Value.ToString().ToUpper() == InputStr.ToUpper())
                {
                    foreach (XmlNode childnode in xnode.ChildNodes)
                    {
                        OutputStr += $"{childnode.InnerText}" + "\n";
                    }

                }
            }

            if (OutputStr == null)
            {
                OutputStr = "Ничего не найдено. Попробуйте изменить запрос";
            }

            Embed embed;
            var EmbedBuilder = new EmbedBuilder()
                   .WithColor(Color.Teal)
                   .WithFooter(footer =>
                   {
                       footer
                        .WithText(OutputStr);
                   });
            embed = EmbedBuilder.Build();

            await ReplyAsync(embed: embed);
        }

        [Command("tool")]
        [Alias("Tool", "t", "T", "И", "и", "инструмент", "Инструмент")]
        public async Task Toolkits(string InputStr)
        {

            string OutputStr = null;
            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(Application.StartupPath + @"\DB\toolkits.xml");

            XmlElement xRoot = xDoc.DocumentElement;
            foreach (XmlNode xnode in xRoot)
            {
                if (xnode.Attributes.Item(0).Value.ToString().ToUpper() == InputStr.ToUpper())
                {
                    foreach (XmlNode childnode in xnode.ChildNodes)
                    {
                        OutputStr += $"{childnode.InnerText}" + "\n";
                    }

                }
            }

            if (OutputStr == null)
            {
                OutputStr = "Ничего не найдено. Попробуйте изменить запрос";
            }

            Embed embed;
            var EmbedBuilder = new EmbedBuilder()
                   .WithColor(Color.Teal)
                   .WithFooter(footer =>
                   {
                       footer
                        .WithText(OutputStr);
                   });
            embed = EmbedBuilder.Build();

            await ReplyAsync(embed: embed);
        }

    }
}

