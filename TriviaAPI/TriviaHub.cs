using Microsoft.AspNetCore.SignalR;
using System.Xml.Linq;

namespace TriviaAPI
{
    public class TriviaHub : Hub
    {
        private static int currentQuestion = 0;
        private static Dictionary<string, int> scores = new Dictionary<string, int>();
        private static readonly List<string> Questions = new List<string>
        {
            "What color is the sky?",
            "Which animal says 'meow'?",
            "How many legs does a dog have?",
            "What shape is a circle?",
            "What do you use to eat soup?"
        };

        private static readonly List<string> CorrectAnswers = new List<string>
        {
            "B",
            "A",
            "C",
            "A",
            "D"
        };

        private static readonly List<List<string>> Options = new List<List<string>>
        {
            new List<string> { "Green", "Blue", "Red", "Yellow" },
            new List<string> { "Cat", "Dog", "Bird", "Elephant" },
            new List<string> { "2", "6", "4", "8" },
            new List<string> { "Round", "Square", "Triangle", "Star" },
            new List<string> { "Fork", "Spoon", "Knife", "Bowl" }
        };

        public async Task Register(string name)
        {
            Context.Items["username"] = name;
            await Clients.Caller.SendAsync("ReceiveQuestion", Questions[currentQuestion], Options[currentQuestion]);
        }

        public async Task<bool> SubmitAnswer(string answer)
        {
            var username = Context.Items["username"] as string;

            bool isCorrect = answer == CorrectAnswers[currentQuestion];

            // Update the score if the answer is correct
            if (isCorrect)
            {
                if (!scores.ContainsKey(username))
                {
                    scores[username] = 0;
                }
                scores[username]++;
            }

            // Send the result to the client
            await Clients.Caller.SendAsync("ReceiveAnswerResult", isCorrect, CorrectAnswers[currentQuestion]);

            return isCorrect;
        }

        public async Task NextQuestion()
        {
            currentQuestion++;
            if (currentQuestion < Questions.Count)
            {
                await Clients.All.SendAsync("ReceiveQuestion", Questions[currentQuestion], Options[currentQuestion]);
            }
            else
            {
                await Clients.All.SendAsync("EndGame", scores);
            }
        }

        public async Task Restart()
        {
            // Reset the game data
            currentQuestion = 0;
            scores.Clear();

            // Notify all clients that the game has been restarted
            await Clients.All.SendAsync("GameRestarted");
        }
    }
}