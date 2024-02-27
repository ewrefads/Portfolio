from Components import Component
import pygame
import multiprocessing

class MovingPlatform(Component):

    speed = 0

    active = False

    endpoint_delay = 0

    remaining_delay = 0

    startpoint = None

    endpoint = None

    current_destination = None

    player = None

    used_by_player = False

    def __init__(self, speed, active, endpoint_delay, startpoint, endpoint) -> None:
        self.speed = speed
        self.active = active
        self.endpoint_delay = endpoint_delay
        self.startpoint = startpoint
        self.endpoint = endpoint
        self.current_destination = endpoint
        
        self.gameworld = None
        super().__init__()
        
   
    def value(self):
        print("enter endpos: fx 200 300")
        return {
            'startpos': [self.startpoint.x, self.startpoint.y]
        }

    
    def awake(self, game_world):
        self.player = game_world.player
        
        self.gameworld = game_world
    def start(self):
        pass
    
    def update(self, delta_time):
        if(self.player is None):
            self.player = self.gameworld.player
            return
            
        
        pos = self._gameObject.transform.position
        player_pos = self.player._gameObject.transform.position.copy()
        player_pos.x += self.player._gameObject.get_component("SpriteRenderer").sprite.rect.w
        r = self._gameObject.get_component("SpriteRenderer").sprite.rect
        if self.used_by_player == False:
            if player_pos.x > pos.x and player_pos.x < pos.x + r.w and abs(player_pos.y - pos.y) < 80:
                print("used by player")
                self.used_by_player = True
        if self.used_by_player:
            if player_pos.x < pos.x or player_pos.x > pos.x + r.w or abs(player_pos.y - pos.y) > 80:
                print("not used by player")
                self.used_by_player = False
        if self.remaining_delay == 0 and self.active:
            if self.current_destination == self._gameObject.transform.position:
                self.remaining_delay = self.endpoint_delay
                if self.current_destination == self.startpoint:
                    self.current_destination = self.endpoint
                else:
                    self.current_destination = self.startpoint
            else:
                move = self._gameObject.transform.position.move_towards(self.current_destination, self.speed)
                delta_move = move - self._gameObject.transform.position
                self._gameObject.transform.position = move
                
                if self.used_by_player:
                    self.player.direction.x += delta_move.x
                    self.player.direction.y += delta_move.y
                    print(self.player.direction)
        else:
            self.remaining_delay -= 1

    def toggle(self):
        if self.active:
            self.active = False
        else:
            self.active = True