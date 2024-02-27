from Builder import Builder
from GameObject import GameObject
import pygame
from Components import SpriteRenderer
from UI import Button

class OptionsBuilder(Builder):
    def __init__(self, gameworld) -> None:
        self._gameObject = GameObject(pygame.Vector2(0,0))
        self._gameworld = gameworld
        
    
    def build(self):
        #options gameobject
        screen_size = ((self._gameworld.screen_size))
        
        spawn_pos = pygame.Vector2(
            screen_size[0]-100,
            0
        )
        
        options_button = GameObject(spawn_pos)
        
        ob_sr = options_button.add_component(SpriteRenderer("options.png"))
        ob_sr.follows_camera=True
        ob_sr.set_scale(0.1,0.1)
        
        options_button.add_component(Button(self._gameworld,self.toggle_options))
        
        
        #options window background
        options_window = GameObject(pygame.Vector2(
            screen_size[0]/2-100,
            screen_size[1]/2-200
        ))
        ow_sr = options_window.add_component(SpriteRenderer("whiteSquare.png"))
        ow_sr.follows_camera=True
        ow_sr.set_scale(1,1.5)
        
        #finder i bunden af det vindue alle knapperne sidder i
        window_scale = ow_sr.get_scale()
        
        #en position som definere i midten af vinduets sprite
        middle_anchored_position = pygame.Vector2(
            window_scale[0]/2,
            window_scale[1]/2
        )
        
        #menu button
        menu_button = GameObject(pygame.Vector2(
            middle_anchored_position[0]-25,
            middle_anchored_position[1]-75
        ))
        mb_sr = menu_button.add_component(SpriteRenderer("menu.png"))
        mb_sr.follows_camera=True
        mb_sr.set_scale(0.2,0.2)
        menu_button.add_component(Button(self._gameworld,self.menu_button))
        
        
        #restart level button
        restart_button = GameObject(pygame.Vector2(
            middle_anchored_position[0]-25,
            middle_anchored_position[1]
        ))
        
        rb_sr = restart_button.add_component(SpriteRenderer("restart.jpg"))
        rb_sr.follows_camera=True
        rb_sr.set_scale(0.2,0.2)
        restart_button.add_component(Button(self._gameworld,self.restart_level))
        
        #next level button
        next_button = GameObject(pygame.Vector2(
            middle_anchored_position[0]-25,
            middle_anchored_position[1]+75
        ))
        
        nb_sr = next_button.add_component(SpriteRenderer("next.jpg"))
        nb_sr.follows_camera=True
        nb_sr.set_scale(0.2,0.2)
        next_button.add_component(Button(self._gameworld,self.next_level))
        
        #adds objekts to child of options
        self._gameObject.add_child(options_button, "options button")
        self._gameObject.add_child(options_window, "options window")
        
        #adds object to child of options window
        options_window.add_child(menu_button, "menu button")
        options_window.add_child(restart_button, "restart button")
        options_window.add_child(next_button, "next button")
        
        options_window.is_active = False

    def get_gameObject(self) -> GameObject:
        return self._gameObject

    #toggler om options vinduet er åbent
    def toggle_options(self):
        window = self._gameObject.get_child("options window")
        window.is_active = not window.is_active
        
    #loader menuen
    def menu_button(self):
        self._gameworld._editor.load_menu()
    
    #loader det samme level igen
    def restart_level(self):
        self._gameworld._editor.restart_level()
    
    #loader det næste level hvis der er et
    def next_level(self):
        self._gameworld._editor.next_level()