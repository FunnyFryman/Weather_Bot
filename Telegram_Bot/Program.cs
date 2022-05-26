using System;
using System.Collections.Generic;
using Telegram.Bot; // импортируем пространство имён 
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using System.IO;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Net;
using Newtonsoft.Json;
using System.Text;




namespace WeatherBot
{
    class Program
    {
        private static string token { get; set; } = "5399074525:AAEqmw5VVfa25OehIvfZP7tpIu5gEhThfUI";   // в тип string вписываем токен  , get и set чтобы ключ имел статус только для чтения                                                  
        private static TelegramBotClient client;
        
        static string NameCity;
        static float tempOfCity;
        static string nameOfCity;

        static string answerOnWether;

        
         
        static void Main(string[] args)
        {
            client = new TelegramBotClient(token) { Timeout = TimeSpan.FromSeconds(10) };
            client.GetMeAsync();
            client.StartReceiving(); 
            client.OnMessage += OnMessageHandler;
            Console.ReadLine(); 
        
            client.StopReceiving(); 
        }
        private static async void OnMessageHandler(object sender, MessageEventArgs e) 
        {
            var message = e.Message;
            var me = client.GetMeAsync().Result;
            Console.WriteLine($"Bot_Id: {me.Id} \nBot_Name: {me.FirstName} ");
            if (message.Type == MessageType.Text) 
            {
                NameCity = message.Text;
                bool resp = Weather(NameCity);
                Celsius(tempOfCity);
                string Temp = $"{answerOnWether} \n\nТемпература в {nameOfCity}: {Math.Round(tempOfCity)} °C";
                string NotTemp = "Неправильный город";
                await client.SendTextMessageAsync(message.Chat.Id, resp ? Temp : NotTemp);
                Console.WriteLine($"Пришло сообщение c текстом: {message.Text}"); 
            }
        }
        public static bool Weather(string CityName)
        {
            try
            {
                string url = "https://api.openweathermap.org/data/2.5/weather?q=" + CityName + "&unit=metric&appid=0ab306661a60ca3ae5f476f135829ce9";
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest?.GetResponse();
                string response;

                using (StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream()))
                {
                    response = streamReader.ReadToEnd();
                }
                WeatherResponse weatherResponse = JsonConvert.DeserializeObject<WeatherResponse>(response);

                nameOfCity = weatherResponse.Name;
                tempOfCity = weatherResponse.Main.temp - 272;
                return true;
            }
            catch (System.Net.WebException)
            {
                Console.WriteLine("Возникло исключение");
                nameOfCity = "Неправильный город";
                tempOfCity = 0 ;
                return false;
            }
        }
        public static void Celsius(float celsius)
        {
            if (celsius <= 10)
                answerOnWether = "Сегодня холодно, желательно одеться потеплее";
            else
                answerOnWether = "Сегодня жарко, так что можно выйти в шортах";
        }
    }
}

