from Builder import Builder
from GameObject import GameObject
import pygame 
from Components import SpriteRenderer
from UI import Button, Text

class DeathWindowBuilder:
    def __init__(self, gameworld) -> None:
        super().__init__()
        self._gameworld = gameworld
        self._gameobject = GameObject(pygame.Vector2(0,0))
        
    def build(self):
        
        #screensize
        screen_size = ((self._gameworld.screen_size))

        # window background
        score_window = GameObject(pygame.Vector2(
            screen_size[0]/2-350,
            screen_size[1]/2-200
        ))
        ow_sr = score_window.add_component(SpriteRenderer("whiteSquare.png"))
        ow_sr.follows_camera=True
        ow_sr.set_scale(3,1.5)
        
        
        #finder i bunden af det vindue alle knapperne sidder i
        window_scale = ow_sr.get_scale()
        
         #level complete text
        top_pos = pygame.Vector2(
            window_scale[0]/2,
            50
        )
        level_done = GameObject(top_pos)
        level_done.add_component(Text("YOU DIED!!!", 80, ""))
        
        score_window.add_child(level_done,"level_done text")
        
        #score text
        top_pos = pygame.Vector2(
            window_scale[0]/2,
            100
        )
        
        score_text = GameObject(top_pos)
        score_text.add_component(Text(f"{self._gameworld.score.currentscore}", 50, "SCORE: "))
        
        score_window.add_child(score_text, "score text")
        
        #en position som definere i midten af vinduets sprite
        middle_anchored_position = pygame.Vector2(
            window_scale[0]/2,
            window_scale[1]/2
        )
        
        #menu button
        menu_button = GameObject(pygame.Vector2(
            middle_anchored_position[0]-75,
            middle_anchored_position[1]+75
        ))
        mb_sr = menu_button.add_component(SpriteRenderer("menu.png"))
        mb_sr.follows_camera=True
        mb_sr.set_scale(0.2,0.2)
        menu_button.add_component(Button(self._gameworld,self.menu_button))
        
        
        #restart level button
        restart_button = GameObject(pygame.Vector2(
            middle_anchored_position[0]-0,
            middle_anchored_position[1]+75
        ))
        
        rb_sr = restart_button.add_component(SpriteRenderer("restart.jpg"))
        rb_sr.follows_camera=True
        rb_sr.set_scale(0.2,0.2)
        restart_button.add_component(Button(self._gameworld,self.restart_level))
        
        #next level button
        next_button = GameObject(pygame.Vector2(
            middle_anchored_position[0]+75,
            middle_anchored_position[1]+75
        ))
        
        nb_sr = next_button.add_component(SpriteRenderer("next.jpg"))
        nb_sr.follows_camera=True
        nb_sr.set_scale(0.2,0.2)
        next_button.add_component(Button(self._gameworld,self.next_level))
        
        #adds objekts to child of options
        self._gameobject.add_child(score_window, "score window")
        
        #adds object to child of options window
        score_window.add_child(menu_button, "menu button")
        score_window.add_child(restart_button, "restart button")
        score_window.add_child(next_button, "next button")
        
        score_window.is_active = True

    def get_gameObject(self) -> GameObject:
        return self._gameobject


     #loader menuen
    def menu_button(self):
        self._gameworld._editor.load_menu()
    
    #loader det samme level igen
    def restart_level(self):
        self._gameworld._editor.restart_level()
    
    #loader det næste level hvis der er et
    def next_level(self):
        self._gameworld._editor.next_level()