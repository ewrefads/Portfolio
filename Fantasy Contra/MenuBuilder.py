from Builder import Builder
from GameObject import GameObject
import pygame
from Components import SpriteRenderer
from UI import Button, Text
import sys

class MenuBuilder(Builder):
    def __init__(self, gameworld) -> None:
        super().__init__()
        self._gameworld = gameworld
        self._level_loader = self._gameworld._editor.change_level
        self._screen_size = ((self._gameworld.screen_size))
        
        self._gameobject = None
        
    
    
    def build(self):
        spawn_pos = pygame.Vector2(
            self._screen_size[0]/2,
            self._screen_size[1]/2
        )
        
        #menu parent gameobjekt i midten af skærmen
        self._gameobject = GameObject(spawn_pos)
        
        #levels buttons
        lvl_buttons = []
        spacing = 150
        for i in range(5):
            pos = pygame.Vector2(
                (i * spacing)-spacing*5/2,
                0
            )
            level = GameObject(pos)
            lvl_sr = level.add_component(SpriteRenderer("whiteSquare.png"))
            lvl_sr.follows_camera=True
            lvl_sr.set_scale(0.5,0.5)
            
            middle_pos = lvl_sr.get_scale()

            #tilføjer den til ting
            lvl_buttons.append(level)
            self._gameobject.add_child(level, f"level{i}")
            
            #tilføjer tekst til hver knap
            text = GameObject(pygame.Vector2(middle_pos[0]/2,middle_pos[1]/2))
            text.add_component(Text(f"level {i+1}", 35, ""))
            level.add_child(text, "text")
        
        #giver funktionalitet til knapperne
        lvl_buttons[0].add_component(Button(self._gameworld, self.load_level1))
        lvl_buttons[1].add_component(Button(self._gameworld, self.load_level2))
        lvl_buttons[2].add_component(Button(self._gameworld, self.load_level3))
        lvl_buttons[3].add_component(Button(self._gameworld, self.load_level4))
        lvl_buttons[4].add_component(Button(self._gameworld, self.load_level5))
        
        #overskrift for menuen
        title_pos = pygame.Vector2(
            0,
            -200
        )
        title = GameObject(title_pos)
        title.add_component(Text("WORM ADVENTURES©", 60, ""))
        self._gameobject.add_child(title, "title")
        
        #quit button
        pos = pygame.Vector2(
            -self._screen_size[0]/2,
            self._screen_size[1]/3 -50
        )
        leave = GameObject(pos)
        le_sr = leave.add_component(SpriteRenderer("whiteSquare.png"))
        le_sr.follows_camera=True
        le_sr.set_scale(0.5,0.5)
        middle_pos = le_sr.get_scale()
            
        leave.add_component(Button(self._gameworld, self.quit_game))
        
        leave_text = GameObject(pygame.Vector2(middle_pos[0]/2,middle_pos[1]/2))
        leave_text.add_component(Text("QUIT", 35, ""))
        
        leave.add_child(leave_text, "text")
        self._gameobject.add_child(leave, "quit button")
        
        #audio buttons
        pos = pygame.Vector2(
            self._screen_size[0]/2-100,
            -250
        )
        audio_text = GameObject(pos)
        audio_text.add_component(Text("audio volume", 50, ""))
        self._gameobject.add_child(audio_text, "audio text")
        
        #volume up
        pos = pygame.Vector2(
            self._screen_size[0]/2-150,
            -200
        )
        up = GameObject(pos)
        u_sr = up.add_component(SpriteRenderer("whiteSquare.png"))
        up.add_component(Button(self._gameworld,self.turn_up_volume))
        u_sr.follows_camera=True
        u_sr.set_scale(0.5,0.5)
        middle_pos = u_sr.get_scale()
        
        up_text = GameObject(pygame.Vector2(middle_pos[0]/2,middle_pos[1]/2))
        up_text.add_component(Text("+", 60, ""))
        up.add_child(up_text, "text")
        
        #volume down
        pos = pygame.Vector2(
            self._screen_size[0]/2-150,
            -100
        )
        down = GameObject(pos)
        d_sr = down.add_component(SpriteRenderer("whiteSquare.png"))
        down.add_component(Button(self._gameworld,self.turn_down_volume))
        d_sr.follows_camera=True
        d_sr.set_scale(0.5,0.5)
        middle_pos = d_sr.get_scale()
        
        down_text = GameObject(pygame.Vector2(middle_pos[0]/2,middle_pos[1]/2))
        down_text.add_component(Text("-", 60, ""))
        down.add_child(down_text, "text")
        
        self._gameobject.add_child(up, "volume up")
        self._gameobject.add_child(down, "volume down")

    def get_gameObject(self) -> GameObject:
        return self._gameobject
    
    
    
    
    def load_level1(self):
        self._level_loader(1)
        
    def load_level2(self):
        self._level_loader(2)
        
    def load_level3(self):
        self._level_loader(3)
        
    def load_level4(self):
        self._level_loader(4)
        
    def load_level5(self):
        self._level_loader(5)
        
    def quit_game(self):
        print("quitting game")
        sys.exit()
        
    def turn_down_volume(self):
        self._gameworld.AudioManager.change_volume(-0.1)
        self._gameworld.AudioManager.play_audio('pop')
        print("volume down")
        
    
    def turn_up_volume(self):
        self._gameworld.AudioManager.change_volume(0.1)
        self._gameworld.AudioManager.play_audio('pop')
        print("volume up")