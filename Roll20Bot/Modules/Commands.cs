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
        private string[] txtPack = new string[10] {
            "Отличный бросок",
            "Можно и лучше",
            "Такова судьба",
            "4",
            "5",
            "6",
            "7", 
            "8", 
            "9", 
            "10", 
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
                .WithTitle("Неправильный формат запроса. Пример запроса **xxdyy**, где x - число кубов, а y - кол-во граней.")
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
                    r += random.Next(1, quantityXfacets.Value+1);
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
                    txt = txtPack[random.Next(0, 10)];
                }


                var EmbedBuilder = new EmbedBuilder()
                    .WithColor(Clr)
                    .WithTitle("Бросок - " + r.ToString() + "!")
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
                        .WithText("!r xxxdyyy - бросок кубиков (x - число кубов, а y - кол-во граней)." +
                        "\n!h - памагат" +
                        "\n!book - даст вам ссылку на книжку на гугл диске" +
                        "\n!conv - Конвертер монет из DnD" +
                        "\n что-то может быть когда-нить или вообще ничего не будет и я уничтожу эту строку");
                   });
            embed = EmbedBuilder.Build();
            await ReplyAsync(embed: embed);
        }
    }
}
