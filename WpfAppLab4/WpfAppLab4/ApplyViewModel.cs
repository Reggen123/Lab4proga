using ClassLibraryLab4;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WpfAppLab4
{
    class ApplyViewModel : INotifyPropertyChanged, IObserver
    {
        public EnglishGame englishGame;
        public string CurrentFile;
        private ObservableCollection<WordsInList> words;
        private WordsInList currentwordinlist;
        private string russianWordAdd;
        private string englishWordAdd;

        private string currentGuessAnswer;

        private Visibility gameGuessVisibility = Visibility.Collapsed;
        private Visibility gameTrueOrFalseVisibility = Visibility.Collapsed;
        private Visibility gameMenuVisibility = Visibility.Visible;
        public ApplyViewModel()
        {
            CurrentFile = "";
            words = new ObservableCollection<WordsInList>();
        }

        public void UpdateGameAnswer()
        {
            OnPropertyChanged("CurrentQuestion");
            OnPropertyChanged("CurrentScore");
        }

        public void UpdateGameStart()
        {
            GameMenuVisibility = Visibility.Collapsed;
            if (englishGame.CurrentEnglishGame.Name == "ПравдаИлиЛожь")
                GameTrueOrFalseVisibility = Visibility.Visible;
            if(englishGame.CurrentEnglishGame.Name == "Переведи")
                GameGuessVisibility = Visibility.Visible;
            OnPropertyChanged("CurrentQuestion");
            OnPropertyChanged("CurrentScore");
        }

        public void UpdateGameEnd()
        {
            GameMenuVisibility = Visibility.Visible;
            if (englishGame.CurrentEnglishGame.Name == "ПравдаИлиЛожь")
                GameTrueOrFalseVisibility = Visibility.Collapsed;
            if (englishGame.CurrentEnglishGame.Name == "Переведи")
                GameGuessVisibility = Visibility.Collapsed;
            WindowEndScreen window = new WindowEndScreen($"Игра законченая. Количество очков:{englishGame.CurrentScore}");
            window.Show();
        }

        public bool OpenFile
        {
            get
            {
                return false;
            }
            set
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Multiselect = true;
                openFileDialog.Filter = "Text Files (*.txt)|*.txt|All files (*.*)|*.*";
                openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                if (openFileDialog.ShowDialog() == true)
                {
                    CurrentFile = openFileDialog.FileName;
                    englishGame = new EnglishGame(CurrentFile);
                    englishGame.AddObserver(this);
                    words.Clear();
                    for (int i = 0; i < englishGame.RussianWords.Count; i++)
                    {
                        WordsInList word = new WordsInList($"{englishGame.RussianWords[i]}/{englishGame.EnglishWords[i]}");
                        words.Add(word);
                    }
                    OnPropertyChanged("Words");
                }
            }

        }
        public bool SaveFile
        {
            get
            {
                return false;
            }
            set
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "Text Files (*.txt)|*.txt|All files (*.*)|*.*";
                saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                if (saveFileDialog.ShowDialog() == true)
                {
                    englishGame.SaveGameCurrentWords(saveFileDialog.FileName);
                    OnPropertyChanged("Words");
                }
            }

        }


        public bool AddWords
        {
            get
            {
                return false;
            }
            set
            {
                if (russianWordAdd == "" || englishWordAdd == "")
                    return;
                WordsInList word = new WordsInList($"{russianWordAdd}/{englishWordAdd}");
                words.Add(word);
                englishGame.AddGameWords(russianWordAdd, englishWordAdd);
                OnPropertyChanged("Words");
            }
        }
        public bool DeleteWords
        {
            get
            {
                return false;
            }
            set
            {
                if (currentwordinlist == null)
                    return;
                englishGame.RemoveGameWords(words.IndexOf(currentwordinlist));
                words.Remove(currentwordinlist);
                OnPropertyChanged("Words");
            }
        }


        public string RussianWordAdd
        {
            get
            {
                return russianWordAdd;
            }
            set
            {
                russianWordAdd = value;
            }
        }
        public string EnglishWordAdd
        {
            get
            {
                return englishWordAdd;
            }
            set
            {
                englishWordAdd = value;
            }
        }

        public bool StartGameGuess
        {
            get
            {
                return false;
            }
            set
            {
                if (englishGame == null)
                    return;
                englishGame.CreateGameGuess();
                englishGame.StartGame();
            }
        }
        public bool StartGameTrueOrFalse
        {
            get
            {
                return false;
            }
            set
            {
                if (englishGame == null)
                    return;
                englishGame.CreateGameTrueOrFalse();
                englishGame.StartGame();
            }
        }


        public Visibility GameGuessVisibility
        {
            get
            {
                return gameGuessVisibility;
            }
            set
            {
                gameGuessVisibility = value;
                OnPropertyChanged("GameGuessVisibility");
                OnPropertyChanged("GameGuessEnabled");
            }
        }
        public Visibility GameTrueOrFalseVisibility
        {
            get
            {
                return gameTrueOrFalseVisibility;
            }
            set
            {
                gameTrueOrFalseVisibility = value;
                OnPropertyChanged("GameTrueOrFalseVisibility");
                OnPropertyChanged("GameTrueOrFalseEnabled");
            }
        }
        public Visibility GameMenuVisibility
        {
            get
            {
                return gameMenuVisibility;
            }
            set
            {
                gameMenuVisibility = value;
                OnPropertyChanged("GameMenuVisibility");
                OnPropertyChanged("GameMenuEnabled");
            }
        }



        public bool GameGuessEnabled
        {
            get
            {
                if (gameGuessVisibility == Visibility.Collapsed)
                    return false;
                else
                    return true;
            }
        }
        public bool GameTrueOrFalseEnabled
        {
            get
            {
                if (gameTrueOrFalseVisibility == Visibility.Collapsed)
                    return false;
                else
                    return true;
            }
        }
        public bool GameMenuEnabled
        {
            get
            {
                if (gameMenuVisibility == Visibility.Collapsed)
                    return false;
                else
                    return true;
            }
        }


        public string CurrentScore
        {
            get
            {
                if (englishGame == null || englishGame.CurrentQuestion == englishGame.CurrentEnglishGame.WordsAnswers.Length)
                    return "Текущее количество очков: 0";
                return $"Текущее количество очков:{englishGame.CurrentScore.ToString()}";
            }
        }
        public string CurrentQuestion
        {
            get
            {
                if (englishGame == null || englishGame.CurrentQuestion == englishGame.CurrentEnglishGame.WordsAnswers.Length)
                    return "";
                if (englishGame.CurrentEnglishGame.Name == "ПравдаИлиЛожь")
                {
                    if (englishGame.CurrentEnglishGame.AdditionalSetting[(int)englishGame.CurrentQuestion] == (byte)1)
                    {
                        string rword = englishGame.CurrentEnglishGame.RussianWords[(int)englishGame.CurrentQuestion];
                        string eword = englishGame.CurrentEnglishGame.EnglishWords[(int)englishGame.CurrentQuestion];
                        return $"Правда ли, что слово: {rword}, переводиться как:{eword}";
                    }
                    else if (englishGame.CurrentEnglishGame.AdditionalSetting[(int)englishGame.CurrentQuestion] == (byte)0)
                    {
                        string rword = englishGame.CurrentEnglishGame.RussianWords[(int)englishGame.CurrentQuestion];
                        string eword = englishGame.CurrentEnglishGame.EnglishWords[(int)englishGame.CurrentQuestion];
                        return $"Не Правда ли, что слово: {rword}, переводиться как:{eword}";
                    }
                }
                else if (englishGame.CurrentEnglishGame.Name == "Переведи")
                {
                    if (englishGame.CurrentEnglishGame.AdditionalSetting[(int)englishGame.CurrentQuestion] == (byte)0)
                    {
                        string rword = englishGame.CurrentEnglishGame.RussianWords[(int)englishGame.CurrentQuestion];
                        return $"Как переводится слово: {rword}";
                    }
                    else if (englishGame.CurrentEnglishGame.AdditionalSetting[(int)englishGame.CurrentQuestion] == (byte)1)
                    {
                        string eword = englishGame.CurrentEnglishGame.EnglishWords[(int)englishGame.CurrentQuestion];
                        return $"Как переводится слово:{eword}";
                    }
                }
                return "Вопроса нет";
            }
        }
        public bool ChooseYes
        {
            get
            {
                return false;
            }
            set
            {
                englishGame.Answer("yes");
            }
        }
        public bool ChooseNo
        {
            get
            {
                return false;
            }
            set
            {
                englishGame.Answer("no");
            }
        }


        public string CurrentAnswerGuess
        {
            get
            {
                return currentGuessAnswer;
            }
            set
            {
                currentGuessAnswer = value;
                OnPropertyChanged("CurrentAnswerGuess");
            }
        }
        public bool MakeAnswerGuess
        {
            get
            {
                return false;
            }
            set
            {
                englishGame.Answer(currentGuessAnswer);
                currentGuessAnswer = "";
                OnPropertyChanged("CurrentAnswerGuess");
            }
        }


        public WordsInList SelectedWordInList
        {
            get
            {
                return this.currentwordinlist;
            }

            set
            {
                this.currentwordinlist = value;
                OnPropertyChanged("SelectedWordInList");
            }
        }
        public ObservableCollection<WordsInList> Words
        {
            get
            {
                return this.words;
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
