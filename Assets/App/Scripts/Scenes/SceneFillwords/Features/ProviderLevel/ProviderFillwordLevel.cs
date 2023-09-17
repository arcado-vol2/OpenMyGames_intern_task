using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using App.Scripts.Scenes.SceneFillwords.Features.FillwordModels;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.VFX;

namespace App.Scripts.Scenes.SceneFillwords.Features.ProviderLevel
{
    public class LevelReader
    {
        public string ReadFile(string fileName)
        {
            TextAsset textAsset = Resources.Load<TextAsset>(fileName);
            return textAsset.text;
        }
    }
    public class ProviderFillwordLevel : IProviderFillwordLevel
    {

        LevelReader reader = new LevelReader();
        private string[] wordsList;
        private string[] circsList;
        private char[] CheckValues(int index)
        {
            string circs = circsList[index];
            string[] parts = circs.Split(' ');
            var wordsIndexes = new int[parts.Length];
            var letterIndexes = new int[parts.Length][];

            int wordsI = 0;
            int letterI = 0;
            for (int partI = 0; partI < parts.Length; partI++)
            {
                if ((partI & 1) != 0)
                {
                    var subParts = parts[partI].Split(';').Select(int.Parse).ToArray();
                    letterIndexes[letterI++] = subParts;
                }
                else
                {
                    if (int.TryParse(parts[partI], out int number))
                    {
                        wordsIndexes[wordsI++] = number;
                    }

                }
            }
            Array.Resize(ref wordsIndexes, wordsI);
            Array.Resize(ref letterIndexes, letterI);
            //Проверки на валидность
            int maxVal = letterIndexes.Length * letterIndexes.Length; 

            HashSet<int> seen = new HashSet<int>(); 

            for (int i = 0; i < letterIndexes.Length; i++)
            {
                if (letterIndexes[i].Length != letterIndexes.Length) 
                {
                    Debug.LogWarning("Матрица букв не квадратная");
                    return null;
                }
                if (letterIndexes[i].Length != wordsList[wordsIndexes[i]].Length - 1)
                {
                    Debug.LogWarning("Размеры индексов и слова не совпадают");
                    return null;
                }

                for (int j = 0; j < letterIndexes[i].Length; j++)
                {
                    int val = letterIndexes[i][j]; 

                    if (val >= maxVal)
                    {
                        Debug.LogWarning($"Индекс {val} выходит за границы матрицы");
                        return null;
                    }
                    if (seen.Contains(val))
                    {
                        Debug.LogWarning($"Индекс {val} уже занят");
                        return null;
                    }

                    seen.Add(val); 
                }
            }

            /* Случай, когда остаются путсые клетки не обрабатывается, так как он может возникнуть
             * только тогда, когда размер слова из словаря не совпадает с размером из уровня 
             или когда две буквы ссылаются на одинаковую клетку*/


            int levelGridSize = letterIndexes.Length;
            char[] letterGrid = new char[levelGridSize * levelGridSize];

            for (int i = 0; i < levelGridSize; i++)
            {
                for (int j = 0; j < levelGridSize; j++)
                {
                    letterGrid[letterIndexes[i][j]] = wordsList[wordsIndexes[i]][j];
                }
            }
            return letterGrid;
        }
        public GridFillWords LoadModel(int index)
        {
            wordsList = reader.ReadFile("FillWords/words_list").Split('\n');
            circsList = reader.ReadFile("FillWords/pack_0").Split('\n');
            int indexFixed = index - 1;
            char[] letters = { };
            for (int i =0; i < circsList.Length; i++)
            {
                letters = CheckValues(i);
                if (letters != null)
                {
                    index--;
                    if (index == 0)
                    {
                        break;
                    }
                }
            }
            if (index > 0)
            {
                throw new Exception("не удалось загрузить не один уровень");
            }
            int levelGridSize = (int)math.sqrt(letters.Length);
            Vector2Int size = new Vector2Int(levelGridSize, levelGridSize);
            GridFillWords grid = new GridFillWords(size);

            for (int i = 0; i < levelGridSize; i++)
            {
                for (int j = 0; j < levelGridSize; j++)
                {
                    char character = letters[i* levelGridSize + j];
                    CharGridModel charGridModel = new CharGridModel(character);
                    grid.Set(i, j, charGridModel);
                }
            }

            return grid;
        }
        
    }
}
