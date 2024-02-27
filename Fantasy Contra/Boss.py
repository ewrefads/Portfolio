from Entity import Entity

class Boss(Entity):
    def __init__(self, health, speed) -> None:
        super().__init__(health, speed)
        self._state = BossMovement()
        
    
    def awake(self, game_world):
        super().awake(game_world)
    
    def start(self):
        super().start()
    
    def update(self, delta_time):
        if(self._state is not None):
            self._state.update()
    
    def change_state(self, new_state):
        self._state.exit()
        self._state = new_state
        self._state.enter()
    
class BossMovement:
    def enter(self):
        pass
    
    def update(self):
        pass
    
    def exit(self):
        pass
    
class BossAttack:
    def enter(self):
        pass
    
    def update(self):
        pass
    
    def exit(self):
        pass

class BossStunned:
    def enter(self):
        pass
    
    def update(self):
        pass
    
    def exit(self):
        pass