from Builder import Builder
from GameObject import GameObject
from UI import HUD, Text, Slider 
from Components import SpriteRenderer
import pygame

class HUD_Builder(Builder):
    def __init__(self, gameworld) -> None:
        self._gameworld = gameworld
        self.object = None
    
    def build(self):
        #HUD
        display = GameObject(pygame.Vector2(0,0))
        hud = display.add_component(HUD(self._gameworld))
        
        #score text
        scoreText = GameObject(pygame.Vector2(250, 40))
        scoreText.add_component(Text("0", 70, "SCORE: "))
        
        #image with a text (health)
        health_image = GameObject(pygame.Vector2(10,10))
        heart = health_image.add_component(SpriteRenderer("Heart.png"))
        heart.set_scale(0.2,0.2)
        heart.follows_camera=True
        heart.set_sprite_layer(0)
        heart.change_color((255,0,0,255))
        
        heart_text_position = heart.get_scale()
        
        health_text = GameObject(pygame.Vector2(heart_text_position[0]/2,heart_text_position[1]/2))
        health_text.add_component(Text("3", 60,""))
        health_image.add_child(health_text, "health text")
        
        #slider (mana)
        screen_size = ((self._gameworld.screen_size))
        slider = GameObject(pygame.Vector2(10,screen_size[1]-300))
        s = slider.add_component(Slider(50,100, self._gameworld))
        s.slider_setup()
        
        #options button
        
        
        hud.add_ui(scoreText, "score text")
        hud.add_ui(health_image, "health image")
        hud.add_ui(slider, "mana slider")
        
        self.object = display
        
        

    def get_gameObject(self) -> GameObject:
        return self.object