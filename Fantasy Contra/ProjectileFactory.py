from Components import Collider, SpriteRenderer
from Factory import Factory
from enum import Enum
from GameObject import GameObject
import pygame

projectiles = Enum('projectiles', ['firebolt'])
class ProjectileFactory(Factory):


    def __init__(self) -> None:
        super().__init__()

    def Create(self, type):
        projectile =  GameObject(pygame.Vector2(0,0))
        if type == "firebolt":
            projectile.add_component(SpriteRenderer("fireball6.png"))
        elif type == "frostbolt":
            projectile.add_component(SpriteRenderer("frostbolt.png"))
        projectile.add_component(Collider())
        return projectile