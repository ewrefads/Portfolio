from abc import ABC, abstractclassmethod
from GameObject import GameObject
from Components import Animator
from Components import SpriteRenderer
from Components import Collider
import pygame
import random

class Builder(ABC):

    @abstractclassmethod
    def build(self):
        pass

    def get_gameObject(self) -> GameObject:
        pass
