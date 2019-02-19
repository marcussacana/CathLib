using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CathLib {
    internal static class Remap {

        static char[,] Map = new char[,] {
                { '￣', ' '},
                { 'ら', 'é'},
                { 'り', 'ê'},
                { 'む', 'à'},
                { 'も', 'â'},
                { 'エ', 'û'},
                { 'よ', 'è'},
                { 'ろ', 'í'},
                { 'な', 'Ê'},
                { 'め', 'á'},
                { 'た', '¿'},
                { 'ぁ', '¡'},
                { 'ゑ', 'ñ'},
                { 'ん', 'ó'},
                { 'や', 'ä'},
                { 'ォ', 'ü'},
                { 'ィ', 'ö'},
                { 'み', 'ß'},
                { 'ウ', 'ù'},
                { 'れ', 'ì'}
        };

        internal static string Decode(string String) {
            for (int i = 0; i <= Map.GetUpperBound(0); i++)
                String = String.Replace(Map[i, 0], Map[i, 1]);

            return String;
        }
        internal static string Encode(string String) {
            for (int i = 0; i <= Map.GetUpperBound(0); i++)
                String = String.Replace(Map[i, 1], Map[i, 0]);

            return String;
        }

        internal static char Decode(char Char) {
            for (int i = 0; i <= Map.GetUpperBound(0); i++)
                if (Char == Map[i, 0])
                    return Map[i, 1];

            return Char;
        }

        internal static char Encode(char Char) {
            for (int i = 0; i <= Map.GetUpperBound(0); i++)
                if (Char == Map[i, 1])
                    return Map[i, 0];

            return Char;
        }
    }
}
