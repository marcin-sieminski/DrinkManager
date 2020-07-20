﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace BLL
{
    public class DrinkLoader
    {
        private List<Drink> _drinks;

        public DrinkLoader(List<Drink> drinks)
        {
            _drinks = drinks;
        }

        public void AddDrinksFromFile(string path = null)
        {
            List<Drink> newDrinks;

            try
            {
                newDrinks = LoadFromFile(path);
            }
            catch (Exception e)
            {
                Console.WriteLine($"\n{e.Message}\n");
                return;
            }

            if (_drinks != null)
            {
                _drinks.AddRange(newDrinks);
            }
            else
            {
                _drinks = newDrinks;
            }
        }

        private List<Drink> LoadFromFile(string path = null)
        {
            if (path == null)
            {
                path = GetPath();
            }

            var drinksString = File.ReadAllText(path);
            var newDrinks = JsonConvert.DeserializeObject<List<Drink>>(drinksString);

            return newDrinks;
        }

        private static string GetPath()
        {
            Console.WriteLine("Please provide full path to the file with new drinks");
            var path = Console.ReadLine();
            return path;
        }
    }
}