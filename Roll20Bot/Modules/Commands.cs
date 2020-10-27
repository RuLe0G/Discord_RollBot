using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;

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
                    if (i != quantityXfacets.Key -1)
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
                        .WithText("!r xxxdyyy - бросок кубиков (x - число кубов, а y - кол-во граней).");
                   });
            embed = EmbedBuilder.Build();
            await ReplyAsync(embed: embed);
        }
    }
}

