
from Components import Component
import pygame


class Camera(Component):
    def __init__(self, is_editor_camera, gameworld) -> None:
        super().__init__()
        self._following_object = None
        self._is_editor_camera = is_editor_camera
        self._camera_speed = 20
        
        self._gameworld = gameworld
        
        
    @property
    def following_object(self):
        return self._following_object
    
    @following_object.setter
    def following_object(self,value):
        #vi følger ikke længere det gamle objekt
        #if(self._following_object is not None):
            #self._all_sprites.remove(self._following_object)
        
        #følger nyt objekt
        self._following_object = value
        
        #vi følger nyt objekt
        #self._all_sprites.add(value)
    
    def awake(self, game_world):
        pass    
    
    def start(self):
        pass
    
    def get_input(self):
        x = 0
        y = 0
        
        keys = pygame.key.get_pressed()
        
        if keys[pygame.K_LEFT]:
            x = -self._camera_speed
        elif keys[pygame.K_RIGHT]:
            x = self._camera_speed
            
        if keys[pygame.K_UP]:
            y = -self._camera_speed
        elif keys[pygame.K_DOWN]:
            y = self._camera_speed
   
        return pygame.Vector2(x,y)
    
    def update(self, delta_time):
        direction = pygame.Vector2(0,0)
        
        #hvis det er et editor kamera så bevæger det sig ud fra spiller input
        if(self._is_editor_camera):
            direction = self.get_input()
            self.gameObject.transform.translate(direction)
        
        #hvis det ikke er et editor kamera så vil den følge et objekt
        else:
            if(self._following_object is not None):
                screenoffset = self._gameworld.screen_size /2
                self.gameObject.transform.position = self._following_object.transform.position - screenoffset
                