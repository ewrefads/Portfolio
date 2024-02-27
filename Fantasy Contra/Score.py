from Components import dir_prefix

class Score:
    highscores = []
    currentscore = 0
    
    def __init__(self, game_world) -> None:
        self.game_world = game_world
        f = open(dir_prefix+"scores.txt", "r")
        for line in f:
            self.highscores.append(int(line))
        f.close()

    def change_score(self, value):
        self.currentscore += value

    def add_to_highscores(self):
        self.highscores.append(self.currentscore)
        f = open(dir_prefix+"scores.txt", "a")
        f.write(f"{self.current_score()} \n")
        f.close()
        self.currentscore = 0

    def get_highscores(self):
        self.highscores.sort()
        return self.highscores

    def current_score(self):
        return self.currentscore