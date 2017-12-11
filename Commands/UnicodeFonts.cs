using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dotbot.Commands
{
    public class UnicodeFonts : ModuleBase<SocketCommandContext>
    {
        public class UnicodeFont
        {
            public string Uppers { get; set; }
            public string Lowers { get; set; }
            public string Nums { get; set; }

            internal string ConvertChar(char c)
            {
                if (c >= '0' && c <= '9')
                    return $"{Nums[c - '0']} ";
                else if (c >= 'a' && c <= 'z')
                    return $"{Lowers[c - 'a']} ";
                else if (c >= 'A' && c <= 'Z')
                    return $"{Uppers[c - 'z']} ";
                return "";
            }

            internal string Convert(string msgtext) => string.Join("", msgtext.ToCharArray().Select(c => ConvertChar(c)));
        }

        public static Dictionary<string, UnicodeFont> Fonts = new Dictionary<string, UnicodeFont>
        {
            ["full"] = new UnicodeFont
            {
                Uppers = "ＡＢＣＤＥＦＧＨＩＪＫＬＭＮＯＰＱＲＳＴＵＶＷＸＹＺ",
                Lowers = "ａｂｃｄｅｆｇｈｉｊｋｌｍｎｏｐｑｒｓｔｕｖｗｘｙｚ",
                Nums = "０１２３４５６７８９",
            },
            ["mono"] = new UnicodeFont
            {
                Uppers = "𝙰𝙱𝙲𝙳𝙴𝙵𝙶𝙷𝙸𝙹𝙺𝙻𝙼𝙽𝙾𝙿𝚀𝚁𝚂𝚃𝚄𝚅𝚆𝚇𝚈𝚉",
                Lowers = "𝚊𝚋𝚌𝚍𝚎𝚏𝚐𝚑𝚒𝚓𝚔𝚕𝚖𝚗𝚘𝚙𝚚𝚛𝚜𝚝𝚞𝚟𝚠𝚡𝚢𝚣",
                Nums = "𝟶𝟷𝟸𝟹𝟺𝟻𝟼𝟽𝟾𝟿",
            },
            ["flipped"] = new UnicodeFont
            {
                Uppers = "ɐqɔpǝɟƃɥıɾʞןɯuodbɹsʇn𐌡ʍxʎz",
                Lowers = "ɐqɔpǝɟƃɥıɾʞןɯuodbɹsʇnʌʍxʎz",
                Nums = "0123456789",
            },
            ["reversed"] = new UnicodeFont
            {
                Uppers = "AdↃbƎꟻGHIJK⅃MᴎOꟼpᴙꙄTUVWXYZ",
                Lowers = "AdↄbɘꟻgHijklmᴎoqpᴙꙅTUvwxYz",
                Nums = "0߁23456789",
            },
            ["cyrillic"] = new UnicodeFont
            {
                Uppers = "αв¢∂єƒﻭнιנкℓмησρ۹яѕтυνωχуչ",
                Lowers = "αв¢∂єƒﻭнιנкℓмησρ۹яѕтυνωχуչ",
                Nums = "0123456789",
            },
            ["slashed"] = new UnicodeFont
            {
                Uppers = "ȺɃȻĐɆFǤĦƗɈꝀŁMNØⱣꝖɌSŦᵾVWXɎƵ",
                Lowers = "Ⱥƀȼđɇfǥħɨɉꝁłmnøᵽꝗɍsŧᵾvwxɏƶ",
                Nums = "01ƻ3456789",
            },
            ["script"] = new UnicodeFont
            {
                Uppers = "𝓐𝓑𝓒𝓓𝓔𝓕𝓖𝓗𝓘𝓙𝓚𝓛𝓜𝓝𝓞𝓟𝓠𝓡𝓢𝓣𝓤𝓥𝓦𝓧𝓨𝓩",
                Lowers = "𝓪𝓫𝓬𝓭𝓮𝓯𝓰𝓱𝓲𝓳𝓴𝓵𝓶𝓷𝓸𝓹𝓺𝓻𝓼𝓽𝓾𝓿𝔀𝔁𝔂𝔃",
                Nums = "𝟎𝟏𝟐𝟑𝟒𝟓𝟔𝟕𝟖𝟗",
            },
            ["gothic"] = new UnicodeFont
            {
                Uppers = "𝕬𝕭𝕮𝕯𝕰𝕱𝕲𝕳𝕴𝕵𝕶𝕷𝕸𝕹𝕺𝕻𝕼𝕽𝕾𝕿𝖀𝖁𝖂𝖃𝖄𝖅",
                Lowers = "𝖆𝖇𝖈𝖉𝖊𝖋𝖌𝖍𝖎𝖏𝖐𝖑𝖒𝖓𝖔𝖕𝖖𝖗𝖘𝖙𝖚𝖛𝖜𝖝𝖞𝖟",
                Nums = "𝟘𝟙𝟚𝟛𝟜𝟝𝟞𝟟𝟠𝟡",
            },
            ["vaporwave"] = new UnicodeFont
            {
                Uppers = "ＡＢＣＤＥＦＧＨＩＪＫＬＭＮＯＰＱＲＳＴＵＶＷＸＹＺ",
                Lowers = "ａｂｃｄｅｆｇｈｉｊｋｌｍｎｏｐｑｒｓｔｕｖｗｘｙｚ",
                Nums = "０１２３４５６７８９",
            },
        };


        [Command("block")]
        [Summary("block text!")]
        public async Task BlockText([Remainder] string text)
        {
            var Nums = new string[] { ":zero:", ":one:", ":two:", ":three:", ":four:", ":five:", ":six:", ":seven:", ":eight:", ":nine:" };
            await ReplyAsync(string.Join("", text.ToCharArray().Select(c => Char.IsDigit(c) ? $"{Nums[c - '0']} " : Char.IsLetter(c) ? $":regional_indicator_{Char.ToLower(c)}: " : "")));
        }


        [Command("fonts")]
        [Alias("fontlist")]
        public async Task ListFonts()
        {
            await base.ReplyAsync($"here are the available unicode fonts:```{string.Join(", ", Fonts.Keys)}```");
        }

    }
}
