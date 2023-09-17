using System;
using System.Collections.Generic;
using App.Scripts.Libs.Factory;
using App.Scripts.Scenes.SceneWordSearch.Features.Level.Models.Level;

namespace App.Scripts.Scenes.SceneWordSearch.Features.Level.BuilderLevelModel
{
    public class FactoryLevelModel : IFactory<LevelModel, LevelInfo, int>
    {
        public LevelModel Create(LevelInfo value, int levelNumber)
        {
            var model = new LevelModel();

            model.LevelNumber = levelNumber;

            model.Words = value.words;
            model.InputChars = BuildListChars(value.words);

            return model;
        }

    private List<char> BuildListChars(List<string> words)
        {
            Dictionary<char, int> lettersCount = new Dictionary<char, int>();

            foreach (string word in words)
            {
                Dictionary<char, int> tmpLettersCount = new Dictionary<char, int>();

                foreach (char letter in word)
                {
                    if (!tmpLettersCount.ContainsKey(letter))
                    {
                        tmpLettersCount[letter] = 1;
                    }
                    else
                    {
                        tmpLettersCount[letter]++;
                    }
                }

                foreach (char letter in tmpLettersCount.Keys)
                {
                    if (!lettersCount.ContainsKey(letter))
                    {
                        lettersCount[letter] = tmpLettersCount[letter];
                    }
                    else if (tmpLettersCount[letter] > lettersCount[letter])
                    {
                        lettersCount[letter] = tmpLettersCount[letter];
                    }
                }
            }

            List<char> chars = new List<char>();
            foreach (char letter in lettersCount.Keys)
            {
                for (int i = 0; i < lettersCount[letter]; i++)
                {
                    chars.Add(letter);
                }
            }

            return chars;
        }

    }
}