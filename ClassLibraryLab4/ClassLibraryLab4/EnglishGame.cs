using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibraryLab4
{
    public class EnglishGame : IObservable
    {
        private List<IObserver> observers;
        public List<string> RussianWords;
        public List<string> EnglishWords;
        public EnglishGameNow CurrentEnglishGame;

        public int? CurrentScore;
        public int? CurrentQuestion;
        public bool IsGameStarted;
        public EnglishGame(string dest)
        {
            OpenGameWords(dest);
            observers = new List<IObserver>();
            IsGameStarted = false;
            CurrentEnglishGame = null;
        }

        public bool StartGame()
        {
            if (CurrentEnglishGame == null)
                return false;
            CurrentScore = 0;
            CurrentQuestion = 0;
            IsGameStarted = true;
            NotifyGameStart();
            return true;
        }

        public bool Answer(string answer)
        {
            if (!IsGameStarted)
                return false;

            if (CurrentEnglishGame.WordsAnswers[(int)CurrentQuestion].ToLower().Replace(" ", "") == answer.ToLower().Replace(" ", ""))
            {
                CurrentScore += CurrentEnglishGame.ScoresForWord[(int)CurrentQuestion];
                CurrentQuestion += 1;
                NotifyGameAnswer();
                if (CurrentQuestion == CurrentEnglishGame.WordsAnswers.Length)
                {
                    IsGameStarted = false;
                    NotifyGameEnd();
                }
                return true;
            }
            else
            {
                CurrentQuestion += 1;
                NotifyGameAnswer();
                if (CurrentQuestion == CurrentEnglishGame.WordsAnswers.Length)
                {
                    IsGameStarted = false;
                    NotifyGameEnd();
                }
                return false;
            }
        }

        public bool EndGame()
        {
            if (CurrentEnglishGame == null)
                return false;
            IsGameStarted = false;
            CurrentEnglishGame = null;
            NotifyGameEnd();
            return true;
        }

        public bool CreateGameTrueOrFalse()
        {
            if (RussianWords == null || EnglishWords == null)
                return false;
            CreateEnglishGame creategame = new CreateEnglishGame();
            EnglishGameBuilder builder = new TrueOrFalseGame();
            CurrentEnglishGame = creategame.CreateGame(builder, RussianWords, EnglishWords);
            return true;
        }

        public bool CreateGameGuess()
        {
            if (RussianWords == null || EnglishWords == null)
                return false;
            CreateEnglishGame creategame = new CreateEnglishGame();
            EnglishGameBuilder builder = new GuessGame();
            CurrentEnglishGame = creategame.CreateGame(builder, RussianWords, EnglishWords);
            return true;
        }

        public bool OpenGameWords(string dest)
        {
            if (!File.Exists(dest))
                return false;
            string[] fileInfo = File.ReadAllLines(dest);
            RussianWords = new List<string>();
            EnglishWords = new List<string>();
            foreach (var w in fileInfo)
            {
                string[] ws = w.Split('/');
                RussianWords.Add(ws[0]);
                EnglishWords.Add(ws[1]);
            }
            return true;
        }

        public void AddGameWords(string rword, string eword)
        {
            if (IsGameStarted)
                return;
            RussianWords.Add(rword);
            EnglishWords.Add(eword);
        }

        public void RemoveGameWords(int index)
        {
            if (IsGameStarted)
                return;
            RussianWords.RemoveAt(index);
            EnglishWords.RemoveAt(index);
        }


        public bool SaveGameCurrentWords(string dest)
        {
            if (File.Exists(dest) || RussianWords.Count != EnglishWords.Count)
                return false;
            string[] contents = new string[RussianWords.Count];
            for (int i = 0; i < RussianWords.Count; i++)
            {
                contents[i] = $"{RussianWords[i]}/{EnglishWords[i]}";
            }
            File.WriteAllLines(dest, contents);
            return true;
        }

        public bool SaveGameWords(string dest, string[] ewords, string[] rwords)
        {
            if (File.Exists(dest) || ewords.Length != rwords.Length)
                return false;
            string[] contents = new string[ewords.Length];
            for (int i = 0; i < ewords.Length; i++)
            {
                contents[i] = $"{rwords[i]}/{ewords[i]}";
            }
            File.WriteAllLines(dest, contents);
            return true;
        }

        public void AddObserver(IObserver o)
        {
            observers.Add(o);
        }

        public void RemoveObserver(IObserver o)
        {
            observers.Remove(o);
        }

        public void NotifyGameStart()
        {
            foreach (IObserver observer in observers)
            {
                observer.UpdateGameStart();
            }
        }

        public void NotifyGameEnd()
        {
            foreach (IObserver observer in observers)
            {
                observer.UpdateGameEnd();
            }
        }

        public void NotifyGameAnswer()
        {
            foreach (IObserver observer in observers)
            {
                observer.UpdateGameAnswer();
            }
        }
    }
    public abstract class EnglishGameBuilder
    {
        public EnglishGameNow EnglishGameNow { get; private set; }
        public List<string> EnglishWords { get; private set; }
        public List<string> RussianWords { get; private set; }
        public void CreateGame(List<string> rwords, List<string> ewords)
        {
            EnglishGameNow = new EnglishGameNow();
            EnglishWords = ewords;
            RussianWords = rwords;
        }
        public abstract void CreateName();
        public abstract void CreateQuestions();
        public abstract void SetScores();
        public abstract void CreaterScores();
        public abstract void SetAdditionalSettings();
        public abstract void SetAnswers();

    }

    public class TrueOrFalseGame : EnglishGameBuilder
    {
        public override void CreateName()
        {
            this.EnglishGameNow.Name = "ПравдаИлиЛожь";
        }
        public override void CreateQuestions()
        {
            Random rnd = new Random();
            this.EnglishGameNow.EnglishWords = EnglishWords.ToArray();
            this.EnglishGameNow.RussianWords = RussianWords.ToArray();
            for (int i = 0; i < RussianWords.Count; i++)
            {
                int randomword = rnd.Next(0, EnglishGameNow.RussianWords.Length);
                string a = EnglishGameNow.RussianWords[i];
                this.EnglishGameNow.RussianWords[i] = EnglishGameNow.RussianWords[randomword];
                this.EnglishGameNow.RussianWords[randomword] = a;
            }
            for (int i = 0; i < EnglishWords.Count; i++)
            {
                int randomword = rnd.Next(0, this.EnglishGameNow.EnglishWords.Length);
                string a = this.EnglishGameNow.EnglishWords[i];
                this.EnglishGameNow.EnglishWords[i] = EnglishGameNow.EnglishWords[randomword];
                this.EnglishGameNow.EnglishWords[randomword] = a;
            }
        }
        public override void SetScores()
        {
            this.EnglishGameNow.ScoreForLetter = 2;
        }

        public override void CreaterScores()
        {
            this.EnglishGameNow.ScoresForWord = new int[this.EnglishGameNow.EnglishWords.Length];
            for (int i = 0; i < this.EnglishGameNow.EnglishWords.Length; i++)
            {
                this.EnglishGameNow.ScoresForWord[i] = this.EnglishGameNow.ScoreForLetter * this.EnglishGameNow.EnglishWords[i].Length;
            }
        }

        public override void SetAdditionalSettings()
        {
            this.EnglishGameNow.AdditionalSetting = new byte[this.EnglishGameNow.EnglishWords.Length];
            Random rnd = new Random();
            for (int i = 0; i < this.EnglishGameNow.AdditionalSetting.Length; i++)
            {
                this.EnglishGameNow.AdditionalSetting[i] = (byte)rnd.Next(0, 2);
            }
        }

        public override void SetAnswers()
        {
            this.EnglishGameNow.WordsAnswers = new string[this.EnglishGameNow.EnglishWords.Length];
            for (int i = 0; i < this.EnglishGameNow.EnglishWords.Length; i++)
            {
                string eword = this.EnglishGameNow.EnglishWords[i];
                string rword = this.EnglishGameNow.RussianWords[i];
                if (EnglishWords.IndexOf(eword) == RussianWords.IndexOf(rword))
                {
                    if (this.EnglishGameNow.AdditionalSetting != null && this.EnglishGameNow.AdditionalSetting[i] == (byte)1)
                        this.EnglishGameNow.WordsAnswers[i] = "yes";
                    else if (this.EnglishGameNow.AdditionalSetting != null && this.EnglishGameNow.AdditionalSetting[i] == (byte)0)
                        this.EnglishGameNow.WordsAnswers[i] = "no";
                }
                else
                {
                    if (this.EnglishGameNow.AdditionalSetting != null && this.EnglishGameNow.AdditionalSetting[i] == (byte)1)
                        this.EnglishGameNow.WordsAnswers[i] = "no";
                    else if (this.EnglishGameNow.AdditionalSetting != null && this.EnglishGameNow.AdditionalSetting[i] == (byte)0)
                        this.EnglishGameNow.WordsAnswers[i] = "yes";
                }
            }
        }
    }

    public class GuessGame : EnglishGameBuilder
    {
        public override void CreateName()
        {
            this.EnglishGameNow.Name = "Переведи";
        }
        public override void CreateQuestions()
        {
            Random rnd = new Random();
            this.EnglishGameNow.EnglishWords = EnglishWords.ToArray();
            this.EnglishGameNow.RussianWords = RussianWords.ToArray();
            for (int i = 0; i < RussianWords.Count; i++)
            {
                int randomword = rnd.Next(0, EnglishGameNow.RussianWords.Length);
                string a = EnglishGameNow.RussianWords[i];
                this.EnglishGameNow.RussianWords[i] = EnglishGameNow.RussianWords[randomword];
                this.EnglishGameNow.RussianWords[randomword] = a;
                string b = EnglishGameNow.EnglishWords[i];
                this.EnglishGameNow.EnglishWords[i] = EnglishGameNow.EnglishWords[randomword];
                this.EnglishGameNow.EnglishWords[randomword] = b;
            }
        }
        public override void SetScores()
        {
            this.EnglishGameNow.ScoreForLetter = 5;
        }

        public override void CreaterScores()
        {
            this.EnglishGameNow.ScoresForWord = new int[this.EnglishGameNow.EnglishWords.Length];
            for (int i = 0; i < this.EnglishGameNow.EnglishWords.Length; i++)
            {
                this.EnglishGameNow.ScoresForWord[i] = this.EnglishGameNow.ScoreForLetter * this.EnglishGameNow.EnglishWords[i].Length;
            }
        }

        public override void SetAdditionalSettings()
        {
            this.EnglishGameNow.AdditionalSetting = new byte[this.EnglishGameNow.EnglishWords.Length];
            Random rnd = new Random();
            for (int i = 0; i < this.EnglishGameNow.AdditionalSetting.Length; i++)
            {
                this.EnglishGameNow.AdditionalSetting[i] = (byte)rnd.Next(0, 2);
            }
        }

        public override void SetAnswers()
        {
            this.EnglishGameNow.WordsAnswers = new string[this.EnglishGameNow.EnglishWords.Length];
            for (int i = 0; i < this.EnglishGameNow.EnglishWords.Length; i++)
            {
                if (this.EnglishGameNow.AdditionalSetting != null && this.EnglishGameNow.AdditionalSetting[i] == (byte)1)
                    this.EnglishGameNow.WordsAnswers[i] = this.EnglishGameNow.RussianWords[i];
                else if (this.EnglishGameNow.AdditionalSetting != null && this.EnglishGameNow.AdditionalSetting[i] == (byte)0)
                    this.EnglishGameNow.WordsAnswers[i] = this.EnglishGameNow.EnglishWords[i];
            }
        }
    }

    public class CreateEnglishGame
    {
        public EnglishGameNow CreateGame(EnglishGameBuilder englishGameBuilder, List<string> rwords, List<string> ewords)
        {
            englishGameBuilder.CreateGame(rwords, ewords);
            englishGameBuilder.CreateName();
            englishGameBuilder.CreateQuestions();
            englishGameBuilder.SetScores();
            englishGameBuilder.CreaterScores();
            englishGameBuilder.SetAdditionalSettings();
            englishGameBuilder.SetAnswers();
            return englishGameBuilder.EnglishGameNow;
        }
    }
    public class EnglishGameNow
    {
        public string Name { get; set; }
        public string[] EnglishWords { get; set; }
        public string[] RussianWords { get; set; }

        public int[] ScoresForWord { get; set; }
        public byte[] AdditionalSetting { get; set; }
        public string[] WordsAnswers { get; set; }
        public int ScoreForLetter { get; set; }
    }
}

