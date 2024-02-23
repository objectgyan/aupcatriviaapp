const connection = new signalR.HubConnectionBuilder()
    .withUrl("https://localhost:7114/trivia")
    .build();

connection.on("ReceiveQuestion", (question, options) => {
    clearOptionClasses();
    document.getElementById("question").innerText = question;
    document.getElementById("optionA").querySelector(".option-text").innerText = options[0];
    document.getElementById("optionB").querySelector(".option-text").innerText = options[1];
    document.getElementById("optionC").querySelector(".option-text").innerText = options[2];
    document.getElementById("optionD").querySelector(".option-text").innerText = options[3];
});

connection.on("EndGame", function (scores) {
    endGame(scores);
});

connection.on("ReceiveAnswerResult", function (isCorrect, answer) {
    // Highlight the correct or wrong answer
    if (isCorrect) {
        document.getElementById('option' + answer).classList.add('correct');
    } else {
        document.getElementById('option' + answer).classList.add('wrong');
    }

    // Disable the options
    document.getElementById('optionA').classList.add('disabled');
    document.getElementById('optionB').classList.add('disabled');
    document.getElementById('optionC').classList.add('disabled');
    document.getElementById('optionD').classList.add('disabled');
});

connection.on("GameRestarted", function () {
    clearOptionClasses();
    document.getElementById('scores').style.display = 'none';
    document.getElementById('scores').innerHTML = '';
});

async function start() {
    try {
        await connection.start();
        console.log("connected");
    } catch (err) {
        console.log(err);
        setTimeout(start, 5000);
    }
};

connection.onclose(start);

// Start the connection.
start();

function register() {
    const name = document.getElementById("name").value;
    connection.invoke("Register", name);
    document.getElementById('game').style.display = 'block';
}

function submitAnswer(answer) {
    // Submit the answer to the server
    connection.invoke("SubmitAnswer", answer).catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
}

function nextQuestion() {
    connection.invoke("NextQuestion");
    document.getElementById('optionA').classList.remove('disabled');
    document.getElementById('optionB').classList.remove('disabled');
    document.getElementById('optionC').classList.remove('disabled');
    document.getElementById('optionD').classList.remove('disabled');
}

function endGame(scores) {
    // Convert the scores object to an array of [username, score] pairs
    let scoresArray = Object.keys(scores).map(function(username) {
        return [username, scores[username]];
    });

    // Sort the scores array by score in descending order
    scoresArray.sort(function(a, b) {
        return b[1] - a[1];
    });

    // Create a string that contains the sorted scores in HTML format
    let scoresHtml = scoresArray.map(function(score) {
        return '<p>' + score[0] + ': ' + score[1] + '</p>';
    }).join('');

    // Update the scores div
    let scoresDiv = document.getElementById('end');

    document.getElementById('scores').innerHTML = scoresHtml;
    scoresDiv.style.display = 'block';
}

function clearOptionClasses() {
    let options = ['optionA', 'optionB', 'optionC', 'optionD'];
    options.forEach(function(optionId) {
        let option = document.getElementById(optionId);
        option.classList.remove('correct');
        option.classList.remove('wrong');
        option.classList.remove('disabled');
    });
}

function restartGame() {
    // Call the Restart method on the server
    connection.invoke("Restart").catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
}