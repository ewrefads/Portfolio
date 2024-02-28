from random import Random

import pygame
from Components import Component
from FlyEyeFactory import FlyEyeFactory
from GoblinFactory import GoblinFactory
from MushroomFactory import MushroomFactory
from SkeletonFactory import SkeletonFactory

class Goal(Component):
    def __init__(self) -> None:
        super().__init__()
        self._gameworld = None
        self.collider = None
    
    def awake(self, game_world):
        self.collider = self._gameObject.get_component("Collider")
        self.collider.subscribe("pixel_collision_enter", self.on_collision_enter)
        self._gameworld = game_world
        
        print("goal redy to goal")
        rnd = Random()
        boss = rnd.randint(0, 3)
        if boss == 0:
            self.boss = GoblinFactory().Create("boss")
            self.boss.transform.position = self.gameObject.transform.position
            self._gameworld.instantiate(self.boss)
        elif boss == 1:
            self.boss = MushroomFactory().Create("boss")
            self.boss.transform.position = self.gameObject.transform.position
            self._gameworld.instantiate(self.boss)
        elif boss == 2:
            self.boss = SkeletonFactory().Create("boss")
            self.boss.transform.position = self.gameObject.transform.position
            self._gameworld.instantiate(self.boss)
        else:
            print("not implemented")
    
    def start(self):
        pass
    
    def update(self, delta_time):
        pass
    
    def on_collision_enter(self, other):

        print(f"collided with {other.gameObject._tag}")
        if(other.gameObject._tag=="player"):

            self._gameworld.level_complete()