using System;
using System.Collections.Generic;
using App.Scripts.Scenes.SceneWordSearch.Features.Level.Models.Level;
using UnityEngine;
namespace App.Scripts.Scenes.SceneWordSearch.Features.Level.BuilderLevelModel.ProviderWordLevel
{
    public class ProviderWordLevel : IProviderWordLevel
    {
        public LevelInfo LoadLevelData(int levelIndex)
        {
            TextAsset jsonResource = Resources.Load<TextAsset>($"WordSearch/Levels/{levelIndex}");
            string jsonData = jsonResource.text;
            LevelInfo levelInfo = JsonUtility.FromJson<LevelInfo>(jsonData);
            return levelInfo;
        }
    }
}