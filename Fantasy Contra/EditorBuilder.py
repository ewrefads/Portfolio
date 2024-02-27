from random import Random
from Builder import Builder
from FlyEyeFactory import FlyEyeFactory
from GameObject import GameObject, GameobjectType
from Components import SpriteRenderer, Collider
from GoblinFactory import GoblinFactory
from MovingPlatform import MovingPlatform
import pygame
from Hazard import Hazard
from Player import Player
from Enemy import Enemy
from Player_Factory import PlayerFactory
from Goal import Goal
from SkeletonFactory import SkeletonFactory
from GoblinFactory import GoblinFactory

class EditorBuilder(Builder):
    
    def __init__(self, gameworld) -> None:
        self._gameworld = gameworld
    
    boss = None


    def build(self, enum, object_dictionary):
        
        values = 0
        
        posX = 0
        posY = 0
        #laver nyt objekt
        if(object_dictionary != None):
            posX = object_dictionary['positionX']
            posY = object_dictionary['positionY']

            values = len(object_dictionary)
                
                
                
        
        gameobject = GameObject(pygame.Vector2(posX,posY))
        gameobject.type = enum
        
        match enum:
            case GameobjectType.GROUND:
                
                gameobject.add_component(SpriteRenderer("grass1.png"))
                
                
                collider = Collider()
                gameobject.add_component(collider)
                
                
            case GameobjectType.MOVINGPLATFORM:
                
                
                
                gameobject.add_component(SpriteRenderer("grass1.png"))
                collider = Collider()
                gameobject.add_component(collider)
                
                
                speed = 3
                active = True
                delay = 2
                startpoint = pygame.Vector2(posX,posY)
                endpoint = pygame.Vector2(0,0)
                
                
                
                #ekstra platform data hvis der er nok data til objecktet
                if(values == 8):
                    speed = object_dictionary['speed']
                    active = object_dictionary['active']
                    delay = object_dictionary['endpointdelay']
                    
                    endpoint = pygame.Vector2(posX + object_dictionary['endpos'][0],posY+object_dictionary['endpos'][1])
                    startpoint = gameobject.transform.position
                else:
                    print(f"using default values: {values}")
                
                gameobject.add_component(MovingPlatform(speed, active, delay, startpoint, endpoint))
            
            
            case GameobjectType.HAZARDS:
                
                #giver hazarden en parent så dens position kan være offset med 25 i spillet
                
                
                hazard = GameObject(pygame.Vector2(0,25))
                
                hazard.add_child(gameobject, "hazard")
                
                
                
                gameobject.add_component(SpriteRenderer("land_mine.png"))
                collider = Collider()
                gameobject.add_component(collider)
                
                
                damage = 5
                speed_reduction = 0.5
                effect_time = 1
                one_time = False
                
                gameobject.add_component(Hazard(damage, speed_reduction, effect_time, one_time))
                hazard.type = enum
                
               
            
            case GameobjectType.ENEMYGOBLIN: 
                gameobject = GoblinFactory().Create("level 0")
                gameobject.transform.position = pygame.Vector2(posX, posY)
                
            case GameobjectType.ENEMYSKELETON:
                gameobject = SkeletonFactory().Create("level 0")
                gameobject.transform.position = pygame.Vector2(posX, posY)
                skeleton_parent = GameObject(pygame.Vector2(0, -11))
                skeleton_parent.add_child(gameobject, "skeleton")
            case GameobjectType.PLAYER:
                
                
                gameobject = self._gameworld.playerFactory.Create("Player")
                gameobject.transform.position = pygame.Vector2(posX, posY)
                gameobject._tag = "player"
                
                player_parent = GameObject(pygame.Vector2(0, -1))
                player_parent.add_child(gameobject, "player")
                
                
            case GameobjectType.GOAL:
                sr = gameobject.add_component(SpriteRenderer("whiteSquare.png"))
                sr.set_scale(0.3,0.3)
                sr.change_color((0,255,0))
                gameobject.add_component(Collider())
                gameobject.add_component(Goal())

                
                
                
                       
        gameobject.type = enum        
        
        sr = gameobject.get_component("SpriteRenderer")
        if(sr is not None):
            sr.awake(self._gameworld)
            
                
        #returns gameobject
        return gameobject
        
    def get_boss(self):
        return self.boss
    def get_gameObject(self) -> GameObject:
        pass
    