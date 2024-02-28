import pygame
from Enemy import Enemy
from GameObject import GameObject
from Components import Animator, Collider
from Components import SpriteRenderer
from GoblinFactory import GoblinFactory
from MushroomFactory import MushroomFactory
from Player import Player
from AudioManager import AudioManager
from LevelEditor import LevelEditor
from GameObject import GameobjectType
from MovingPlatform import MovingPlatform
from Camera import Camera
from Background import Background

from Player_Factory import PlayerFactory
from Score import Score
from OptionsBuilder import OptionsBuilder
from MenuBuilder import MenuBuilder

from ScoreScreenBuilder import ScoreScreenBuilder
from DeathWindowBuilder import DeathWindowBuilder

import os


from EditorBuilder import EditorBuilder

from HUD_Builder import HUD_Builder
from SkeletonFactory import SkeletonFactory
from UI import HUD

from Hazard import Hazard


class GameWorld:

    player = None
    playerFactory = PlayerFactory()
    
    def __init__(self) -> None:
        os.chdir('...')
        pygame.init()



        self._gameObjects = []
        self._UI = []
        self._colliders = []
        self.AudioManager = AudioManager()

        self.score = Score(self)

        #screen
        self._screen_width = 1080
        self._screen_height = 540
        self._screen = pygame.display.set_mode((self._screen_width,self._screen_height), pygame.HWSURFACE | pygame.DOUBLEBUF)
        
        # #initialize the background
        # self.background = Background(self._screen_width, self._screen_height, self._camera) 

        #gameworld logic
        self._running = True
        self._clock = pygame.time.Clock()
        self._tickrate = 100
        
        #editor
        #ændre editormode til true for at edit level
        self._editormode = True
        self._editor = LevelEditor(self)
        
        #kamera
        self._camera = None
        
        #HUD
        self._HUD = None
        self._options = None
        
        #menu
        self._menu = None
        
        #score screen
        self._score_screen = None
        
        #death screen
        self._death_screen = None
        
        
        #bygger test ting
        #self.spawn_test_level()
        
        
        #bygger hud til spillet
        hudbuidler = HUD_Builder(self)
        hudbuidler.build()
        hud_obj = hudbuidler.get_gameObject()
        self._HUD = hud_obj.get_component("HUD")
        self.instantiate_UI(hud_obj)
        
        
        
        #bygger options
        optionsbuilder = OptionsBuilder(self)
        optionsbuilder.build()
        self._options = optionsbuilder.get_gameObject()
        self.instantiate_UI(self._options)
        
        
        #bygger menuen
        menubuilder = MenuBuilder(self)
        menubuilder.build()
        self._menu = menubuilder.get_gameObject()
        self.instantiate_UI(self._menu)
        
        #bygger score endscreen
        scoreScreenBuilder = ScoreScreenBuilder(self)
        scoreScreenBuilder.build()
        self._score_screen = scoreScreenBuilder.get_gameObject()
        self.instantiate_UI(self._score_screen)
        
        #bygger death window
        death_window = DeathWindowBuilder(self)
        death_window.build()
        self._death_screen = death_window.get_gameObject()
        self.instantiate_UI(self._death_screen)
        
        #starter i menu
        self.menu_loaded()
        
        #bygger kameraet    
        self.add_camera()

        #initialize the background
        self.background = Background(self._screen_width, self._screen_height, self._camera)
    
        #editor mode settings
        if(self._editormode):
            self._tickrate = 60
            #awake editor
            self._editor.awake()
            #starts editor
            self._editor.start()
            #late start er en metode der kaldes seperat efter start
            self._editor.late_start()
        #editor mode er disabled
        else:
            #late start er en metode der kaldes seperat efter start
            self._editor.late_start()
         
        
        
        
        
    @property
    def screen(self):
        return self._screen
    
    @property
    def colliders(self):
        return self._colliders
    
    @property
    def camera_offset(self):
        return self._camera.transform.position
    
    @property
    def screen_size(self):
        return pygame.Vector2(self._screen_width, self._screen_height)
    
    def instantiate(self, gameObject):
        gameObject.awake(self)
        gameObject.start()
        self._gameObjects.append(gameObject)
    
    def instantiate_UI(self, gameObject):
        gameObject.awake(self)
        gameObject.start()
        self._UI.append(gameObject)
        
    #metode til at gøre rent i kode når et nyt level bliver loadet ind
    def level_loaded(self):
        print("nyt level loadet")
        
        #finder reference til spiller
        player = self.find_gameobject("player")
        if(player == None):
            eb = EditorBuilder(self)
            player = eb.build(GameobjectType.PLAYER, None)
            self.instantiate(player)
            self._gameObjects.remove(player)
            player._tag = "player"
            self.player = player.get_component("Player")
        else:    
            self.player = player.get_component("Player")
            self._gameObjects.remove(player)
        
       
        
        if(self._editormode):
            self.player.gameObject.make_editor_object()
        
        #koden virker nogen gange ikke uden det her for some reason
        if(self.player.gameObject in self._gameObjects):
            self._gameObjects.remove(self.player.gameObject)
        
        if(self.player.gameObject in self._gameObjects):
            self._gameObjects.remove(self.player.gameObject)
            
        if(self.player.gameObject in self._gameObjects):
            self._gameObjects.remove(self.player.gameObject)
            
            
       
            
        #sætter kamera op
        cam = self._camera.get_component("Camera")
        
        if(not self._editormode):
            cam.following_object = self.player.gameObject
            
        #enable hud
        self._HUD.gameObject.is_active = True
        self._options.is_active = True
        
        #disable menu
        self._menu.is_active = False
        self._score_screen.is_active = False
        self._death_screen.is_active = False
    
    def menu_loaded(self):
        print("loader menu")
        #disble hud
        self._HUD.gameObject.is_active = False
        self._options.is_active = False
        self._score_screen.is_active = False
        self._death_screen.is_active = False
        
        #enable menu
        self._menu.is_active = True
        
    def level_complete(self):
        print("level complete")
        self.playerFactory.damage_mul += 0.5
        self.playerFactory.health += 5
        self.playerFactory.mana += 5
        self._gameObjects = []
        self.menu_loaded()
    
    #finder et objekt i verdnen med et tag
    def find_gameobject(self, tag):
        for obj in self._gameObjects:
            if(obj._tag ==tag and obj.is_active == True):
                return obj
            
        return None
    def update_score(self, value):
        self.score.change_score(value)
        self._HUD.score_change(self.score.current_score())
        
    def update_mana(self, value):
        self._HUD.mana_change(value)
    def spawn_test_level(self):
        player = PlayerFactory().Create("player")
        player.transform.position = pygame.Vector2(1280 / 2, 720 / 2 - 5)
        self._gameObjects.append(player)
        self.player = player.get_component("Player")
        
        enemy = MushroomFactory().Create("level 0")
        enemy.transform.position = pygame.Vector2(1280 / 2 + 80 * 4, 720 /  2 - 190)
        self._gameObjects.append(enemy)

        ground = GameObject(pygame.Vector2(1280 / 2, 720 /  2 + 80))
        ground.add_component(SpriteRenderer("grass1.png"))
        collider = Collider()
        ground.add_component(collider)
        self._gameObjects.append(ground)

        ground = GameObject(pygame.Vector2(1280 / 2 + 80, 720 /  2 - 80))
        ground.add_component(SpriteRenderer("grass1.png"))
        collider = Collider()
        ground.add_component(collider)
        self._gameObjects.append(ground)

        ground = GameObject(pygame.Vector2(1280 / 2 + 80 * 2, 720 /  2 - 80))
        ground.add_component(SpriteRenderer("grass1.png"))
        collider = Collider()
        ground.add_component(collider)
        self._gameObjects.append(ground)
        ground = GameObject(pygame.Vector2(1280 / 2 + 80 * 3, 720 /  2 - 80))
        ground.add_component(SpriteRenderer("grass1.png"))
        collider = Collider()
        ground.add_component(collider)
        self._gameObjects.append(ground)
        ground = GameObject(pygame.Vector2(1280 / 2 + 80 * 3, 720 /  2 - 160))
        ground.add_component(SpriteRenderer("grass1.png"))
        collider = Collider()
        ground.add_component(collider)
        self._gameObjects.append(ground)
        ground = GameObject(pygame.Vector2(1280 / 2 + 80 * 4, 720 /  2 - 80))
        ground.add_component(SpriteRenderer("grass1.png"))
        collider = Collider()
        ground.add_component(collider)
        self._gameObjects.append(ground)
        ground = GameObject(pygame.Vector2(1280 / 2 + 80 * 5, 720 /  2 - 80))
        ground.add_component(SpriteRenderer("grass1.png"))
        collider = Collider()
        ground.add_component(collider)
        self._gameObjects.append(ground)

        

        ground = GameObject(pygame.Vector2(1280 / 2, 720 /  2 - 80))
        ground.add_component(SpriteRenderer("grass1.png"))
        collider = Collider()
        ground.add_component(collider)
        self._gameObjects.append(ground)

        ground = GameObject(pygame.Vector2(1280 / 2 - 80, 720 /  2 - 80))
        ground.add_component(SpriteRenderer("grass1.png"))
        collider = Collider()
        ground.add_component(collider)
        self._gameObjects.append(ground)
        
        
        

        startpoint = pygame.Vector2(1280 / 2 + 80, 720 /  2 + 80)
        moving_platform = GameObject(startpoint)
        moving_platform.add_component(SpriteRenderer("grass1.png"))
        collider = Collider()
        moving_platform.add_component(collider)
        moving_platform.add_component(MovingPlatform(5, True, 10, startpoint, pygame.Vector2(1280 / 2 + 80 * 5, 720 /  2 + 80)))
        self._gameObjects.append(moving_platform)

    def add_camera(self):
        if(self._editormode):
            #sætter kamera op til at bevæge sig på piletasterne
            self._camera = GameObject(pygame.Vector2(0,0))
            cam = Camera(True, self)
            self._camera.add_component(cam)
            cam.following_object = self._editor
            self._gameObjects.append(self._camera)
            #self._all_sprites.add(player_sr)
        
        else:    
            #sætter kamera op til ikke at være editor kamera
            self._camera = GameObject(pygame.Vector2(0,0))
            cam = Camera(False, self)
            self._camera.add_component(cam)
            
            self._gameObjects.append(self._camera)
        
        self._camera._tag="camera"
    
    def Awake(self):
        for gameObject in self._gameObjects[:]:
            gameObject.awake(self)
    
    def Start(self):
        for gameObject in self._gameObjects[:]:
            gameObject.start()
         
        # Afspiller music via. audiomaneger. Brug: self.AudioManeger.stop_audio('audiotest') for at stoppe det
        self.AudioManager.play_audio('audiotest')
        
            
        

    def update(self):
        while self._running:
            for event in pygame.event.get():
                if event.type == pygame.QUIT:
                    self._running =False

            self._screen.fill("cornflowerblue")

            
            
            # Test for afspilning af lydeffekter
            keys = pygame.key.get_pressed()
            if keys[pygame.K_f and not self._editormode]:
                
                self.AudioManager.pause_all_sounds()
            # auto scroll
            self.background.scroll()
            # follow camera
            # self.background.scroll_with_camera()

            
            delta_time = self._clock.tick(self._tickrate) / 1000.0
            
            

            self.background.render(self._screen)            
            #draw your game
            for gameObject in self._gameObjects[:]:
                gameObject.update(delta_time)
                
            
                
            #opdatere leveleditor hvis i editor mode
            if(self._editormode):
                self._editor.update()

            #opdatere spilleren for sig selv så han kan tegnes oven på andre sprites
            if(self.player is not None):
                self.player.gameObject.update(delta_time)
                
            for obj in self._UI[:]:
                obj.update(delta_time)
            
            #collision stuff
            for i, collider1 in enumerate(self._colliders):
                for j in range(i + 1, len(self._colliders)):
                    
                    collider2 = self._colliders[j]
                    collider1.collision_check(collider2)
            self._colliders = [obj for obj in self._colliders if not obj.gameObject.is_destroyed]
            self._gameObjects = [obj for obj in self._gameObjects if not obj.is_destroyed]

            pygame.display.flip()
            
            self._clock.tick(self._tickrate)

        pygame.quit()
    def game_over(self):
        print("the player died")
        self.score.add_to_highscores()
        high_scores = self.score.get_highscores()
        
        self._death_screen.is_active = True
        self.player._health = 10
        
        print("High scores:")
        for i in range(0, len(high_scores)):
            print(f"{i + 1}.  {high_scores[i]}")
        print("game over")
        
    

gw = GameWorld()

gw.Awake()
gw.Start()
gw.update()

    

        