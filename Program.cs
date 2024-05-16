using System.Security.Cryptography;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
namespace BelorusVpered
{
    class Program()
    {
        private static ITelegramBotClient _botClient;
        private static ReceiverOptions _receiverOptions;
        static Dictionary<long, string> equations = new Dictionary<long, string>();
        static Random rnd = new Random();

        #region Program сonstansts
        private const string stepBack = "..//..//..//"; //stepback = "" if release, = "..//..//..//" if debug
        private const string resourcesPath = $"{stepBack}resources//";
        //private const string connectionString = $"Data Source={stepBack}Users_DB.db";
        //private const string NewsChannelLink = "https://t.me/+0gfb7z3CK3c5ODQ0";
        //private const string managerLink = "@Matematicas_support";
        //private const string BotId = "matematicasmxn_bot";
        //private readonly static List<long> adminId = new List<long>() { 1760080161, 6822735004, 948468834 };
        #endregion
        private const string stefan = "Научно-педагогический стаж: 37 лет\r\n\r\nОбразование:\r\n\r\nЛенинградский государственный университет, математико-механический факультет.\nСпециальность: математика.\r\n\r\nДоктор физико-математических наук, профессор по кафедре теоретической и специальной физики. Заслуженный работник НовГУ.";
        private const string marina = "В 1997 году Марина Анатольевна окончила Новгородскую государственную сельскохозяйственную академию по специальности «Зоотехния». Присвоена квалификация «Зооинженер». В 1998 году присуждена ученая степень кандидата сельскохозяйственных наук по специальности 03.00.16 – Экология.\r\nС 1999 года старший преподаватель кафедры животноводства института сельского хозяйства и природных ресурсов НовГУ им. Ярослава Мудрого.\r\nВ 2007 году присвоено ученое звание доцента. С 2011 года работает в должности доцента кафедры биологии и биологической химии.\r\n2012 - 2019 гг. Педагог-организатор кафедры ББХ. Тьютор, супервизор тьюторов. Стипендиат Оксфордской стипендии. С 2020 г. Руководитель тьюторского сопровождения в НовГУ. Член Межрегиональной тьюторской ассоциации, руководитель регионального отделения МТА Новгородской области.";
        private const string shadowraze = "Ученая степень:\tкандидат наук\r\nУченое звание:\tдоцент\r\nОбразование:\tвысшее образование\r\nСпециальность:\tфилолог\r\nАннотация деятельности:\tпреподавание дисциплин кафедры филологии, исследование русской литературы, общественная деятельность.\r\nНаучные интересы:\tЛитературоведение; литература, журналистика, философская мысль русского зарубежья; современная отечественная литература\r\nРабочий график:\tрасписание по кафедре филологии\r\nОбщий стаж:\t28 лет\r\nНаучно-педагогический стаж:\t27 лет";
        private const string igor = "Вся жизнь тесно связана с Великим Новгородом и изучением его исторического прошлого. Родился в Новгороде. Закончил Первую Университетскую гимназию имени академика В.В. Сороки. Затем поступил в Новгородский университет на Исторический факультет, где специализировался по истории России 20 века. Обучение продолжил аспирантуре Университета, по окончанию которой защитил кандидатскую диссертацию. Последние семь лет работал в сфере образования Великого Новгорода. Сначала в Комитете по образованию, затем в Школе №37 Великого Новгорода, в настоящее время исполняю обязанности директора Университетского лицея точных и естественных наук. Свою педагогическую деятельность начинал на историческом факультете Университета, затем продолжил работать со студентами в высших учебных заведениях Великого Новгорода, также имею опыт преподавания в школе.";
        private const string timoxa = "СОСЕТ ХУИ 24 на 7, будьте аккуратны, может загнобить духотой";
        private const string petrova = "Ученая степень: кандидат педагогических наук Ученое звание: доцент по кафедре биологии и биологической химии\r\n\r\nОпыт преподавания: 19 лет\r\n\r\nНаучные и научно-методические труды:\r\n\r\nболее 30 статей, из них 10 – РИНЦ, 7 – ВАК, 6 – Scopus;\r\n2 монографии;\r\nболее 28 методических пособий.\r\nМногократный победитель на семинарах, конференциях и конкурсах Всероссийского и Международного уровня, среди которых конкурс «Образование в России: настоящее и будущее», Международный конкурс педагогического таланта «На вершине научного олимпа» и другие. Так же Екатерина Александровна является членом жюри городского и областного туров Всероссийской олимпиады по химии для школьников, экспертом на защите проектов по биохимии студентов направления подготовки «Биология» и независимым экспертом в IV региональном чемпионате «Молодые профессионалы» (WorldSkills Russia) по компетенции «Аппаратчик химических технологий».";
        #region Keyboards
        public static ReplyKeyboardMarkup menu = new(new[]
        {
            new KeyboardButton[] { "Общая информация о лицее" },
            new KeyboardButton[] { "Список наших преподователей" },
            new KeyboardButton[] { "Внеурочная деятельность" },
            new KeyboardButton[] { "Наши достижения" },
             new KeyboardButton[] { "Контакты администрации лицея" }
        })
        {
            ResizeKeyboard = true
        };
        public static ReplyKeyboardMarkup teachers = new(new[]
        {
            new KeyboardButton[] { "Эминов Стефан Ильич (Учитель математики)" },
            new KeyboardButton[] { "Коновалова Марина Анатольевна (Учитель биологии)" },
            new KeyboardButton[] { "Шадурский Владимир Вячеславович (Учитель литературы)" },
            new KeyboardButton[] { "Толстых Игорь Николаевич (Учитель истории)" },
            new KeyboardButton[] { "Петрова Екатерина Александровна (Учитель химии)" },
            new KeyboardButton[] { "Анисимов Василий Геннадьевич (Учитель физики)" },
            new KeyboardButton[] { "Рябков Тимофей Романович (Учитель духоты)"},
            new KeyboardButton[] { "Назад в меню"}
        })
        {
            ResizeKeyboard = true
        };
        #endregion

        static async Task Main()
        {
            _botClient = new TelegramBotClient("6875254430:AAE96Rtr50D1Gg3HhqgFurHDo6BlQ79-pPg"); 
            _receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = new[]
                {
                    UpdateType.Message,
                    UpdateType.ChatMember,
                },

                ThrowPendingUpdates = true,
            };
            using var cts = new CancellationTokenSource();
            _botClient.StartReceiving(HandleUpdateAsync, HandlePollingErrorAsync, _receiverOptions, cts.Token);
            var me = await _botClient.GetMeAsync();
            Console.WriteLine($"{me.FirstName} is started!");
            await Task.Delay(-1);
            async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
            {
                var message = update.Message;
                var messageText = "";
                long chatId = 0;
                if (message != null)
                {
                    messageText = update.Message.Text;
                    chatId = message.Chat.Id;
                }
                var callback = update.CallbackQuery;
                int answer = 0;
                if (messageText == "/start")
                {
                    await botClient.SendTextMessageAsync(
                                chatId: chatId,
                                text: "Привет, не поступай в лицей, пожалеешь",
                                replyMarkup: menu,
                                cancellationToken: cancellationToken);
                }
                if (messageText == "Общая информация о лицее")
                {
                    await botClient.SendTextMessageAsync(
                                chatId: chatId,
                                text: "Лицей Новгородского государственного университета имени Ярослава Мудрого объединяет классическое образование и новейшие образовательные технологии по двум профилям обучения:\r\n- Технологический\r\n- Естественно-научный\r\n\r\nОтличительной особенностью данного лицея является привлечение профессорско-преподавательского состава НовГУ для реализации образовательной программы среднего общего образования, включение в учебный процесс предметов дополнительного образования университетского уровня, научно- исследовательской и проектной деятельности, системной подготовки к предметным олимпиадам, турнирам и конкурсам, программ\r\nпредпрофессиональной подготовки.\r\n\r\nДополнительное профильное обучение будет осуществляться за счет образовательных ресурсов НовГУ.\r\n\r\nВыбери свой профиль и сделай #Мудрыйвыбор !",
                                replyMarkup: menu,
                                cancellationToken: cancellationToken);
                }
                if (messageText == "Список наших преподователей")
                {
                    await botClient.SendTextMessageAsync(
                                chatId: chatId,
                                text: "Выберите интересующего вас преподователя",
                                replyMarkup: teachers,
                                cancellationToken: cancellationToken);
                }
                if (messageText == "Внеурочная деятельность")
                {
                    await botClient.SendTextMessageAsync(
                                chatId: chatId,
                                text: "НЕ СДЕЛАЛ ЕЩЁ",
                                replyMarkup: menu,
                                cancellationToken: cancellationToken);
                }
                if (messageText == "Наши достижения")
                {
                    await botClient.SendTextMessageAsync(
                                chatId: chatId,
                                text: "НЕ СДЕЛАЛ ЕЩЁ",
                                replyMarkup: menu,
                                cancellationToken: cancellationToken);
                }
                if (messageText == "Контакты администрации лицея")
                {
                    await botClient.SendTextMessageAsync(
                                chatId: chatId,
                                text: "Наша прекрасная завуч - https://t.me/svetlomore \n Наш прекрасный директор - НЕТУ ПОКА КОНТАКТОВ",
                                replyMarkup: menu,
                                cancellationToken: cancellationToken);
                }
                if (messageText == "Эминов Стефан Ильич (Учитель математики)")
                {
                    await using Stream stream = System.IO.File.OpenRead($"{resourcesPath}stefan.jpg");
                    await botClient.SendPhotoAsync(
                                chatId: chatId,
                                photo: InputFile.FromStream(stream), 
                                caption:stefan,
                                replyMarkup: teachers,
                                cancellationToken: cancellationToken);
                }
                if (messageText == "Коновалова Марина Анатольевна (Учитель биологии)")
                {
                    await using Stream stream = System.IO.File.OpenRead($"{resourcesPath}marina.jpg");
                    await botClient.SendPhotoAsync(
                                chatId: chatId,
                                photo: InputFile.FromStream(stream),
                                caption: marina,
                                replyMarkup: teachers,
                                cancellationToken: cancellationToken);
                }
                if (messageText == "Шадурский Владимир Вячеславович (Учитель литературы)")
                {
                    await using Stream stream = System.IO.File.OpenRead($"{resourcesPath}shadowraze.jpg");
                    await botClient.SendPhotoAsync(
                                chatId: chatId,
                                photo: InputFile.FromStream(stream),
                                caption: shadowraze,
                                replyMarkup: teachers,
                                cancellationToken: cancellationToken);
                }
                if (messageText == "Толстых Игорь Николаевич (Учитель истории)")
                {
                    await using Stream stream = System.IO.File.OpenRead($"{resourcesPath}igor.jpg");
                    await botClient.SendPhotoAsync(
                                chatId: chatId,
                                photo: InputFile.FromStream(stream),
                                caption: igor,
                                replyMarkup: teachers,
                                cancellationToken: cancellationToken);
                }
                if (messageText == "Петрова Екатерина Александровна (Учитель химии)")
                {
                    await using Stream stream = System.IO.File.OpenRead($"{resourcesPath}petrova.jpg");
                    await botClient.SendPhotoAsync(
                                chatId: chatId,
                                photo: InputFile.FromStream(stream),
                                caption: petrova,
                                replyMarkup: teachers,
                                cancellationToken: cancellationToken);
                }
                if (messageText == "Анисимов Василий Геннадьевич (Учитель физики)")
                {
                    await using Stream stream = System.IO.File.OpenRead($"{resourcesPath}vasiliy.jpg");
                    await botClient.SendPhotoAsync(
                                chatId: chatId,
                                photo: InputFile.FromStream(stream),
                                caption: igor,
                                replyMarkup: teachers,
                                cancellationToken: cancellationToken);
                }
                if (messageText == "Рябков Тимофей Романович (Учитель духоты)")
                {
                    await using Stream stream = System.IO.File.OpenRead($"{resourcesPath}timoxa.jpg");
                    await botClient.SendPhotoAsync(
                                chatId: chatId,
                                photo: InputFile.FromStream(stream),
                                caption: timoxa,
                                replyMarkup: teachers,
                                cancellationToken: cancellationToken);
                }
                if (messageText == "Назад в меню")
                {
                    await botClient.SendTextMessageAsync(
                        chatId: chatId,
                        text: "Меню",
                        replyMarkup:menu,
                        cancellationToken:cancellationToken);
                }
            }
                Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
                {
                    var ErrorMessage = exception switch
                    {
                        ApiRequestException apiRequestException
                            => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                        _ => exception.ToString()
                    };

                    Console.WriteLine(ErrorMessage);
                    return Task.CompletedTask;
                }
        }
    }
}
