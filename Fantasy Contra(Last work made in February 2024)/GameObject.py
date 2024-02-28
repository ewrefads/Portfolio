import pygame
from Components import Transform
from enum import Enum


class GameObject:

    def __init__(self,position) -> None:
        #super().__init__()
        
        self._components = {}
        self._transform = self.add_component(Transform(position))
        self._is_destroyed = False
        self._is_active = True
        self._type = None
        self._tag =""
        
        #variabler til at have child gameobjekts
        self._children = []
        self._name =""
        self._is_child = False
        self._parent = None

    @property
    def transform(self):
        return self._transform
    
    @property
    def is_destroyed(self):
        return self._is_destroyed
    
    @property
    def components(self):
        return self._components
    
    @property
    def is_active(self):
        return self._is_active
    
    @property
    def type (self):
        return self._type
    
    @type.setter
    def type(self, value):
        self._type = value
    
    
    
    @is_active.setter
    def is_active(self,value):
        self._is_active = value
    
    @property
    def parent(self):
        return self._parent
    
    @parent.setter
    def parent(self,value):
        self._parent = value
    
    
    def destroy(self):
        self._is_destroyed = True



    def add_component(self, component):
        component_name = component.__class__.__name__
        self._components[component_name] = component
        component.gameObject = self
        return component

    def get_component(self, component_name):
        return self._components.get(component_name,None)
    
    def add_child(self, child , name):
        self._children.append(child)
        child._is_child = True
        child.parent = self
        child._name = name
    
    def get_child(self, name):
        for child in self._children:
            if(child._name == name and child._name != ""):
                return child
            
    def get_component_in_children(self, component_name):
        components = []
        for child in self._children:
            component = child._components.get(component_name,None)
            if (component is not None):
                components.append(component)
                
        return components
    
    def get_copy(self, original, game_world):
        copy = GameObject(original.transform.position)
        for component in original.components:
            copy.add_component(original.components[component].get_copy())
        
        copy.awake(game_world)
        copy.type = self._type
        
        return copy
    
    def make_editor_object(self):
        for component in self._components.values():
            if(component.__class__.__name__ == "SpriteRenderer" or component.__class__.__name__ == "Collider"):
                continue
            component._is_active = False
            
    def make_not_editor_object(self):
        for component in self._components.values():
            if(component.__class__.__name__ == "SpriteRenderer" or component.__class__.__name__ == "Collider"):
                continue
            component._is_active = True
    
    def awake(self, game_world):
        #opdater components
        for component in self._components.values():
            component.awake(game_world)
        
        #opdatere child objekts
        for child in self._children:
            child.awake(game_world)

    def start(self):
        #opdater components
        for component in self._components.values():
            component.start()
            
        #opdatere child objekts
        for child in self._children:
            child.start()

    def update(self, delt_time):
        #opdater components og child objekts medmindre dette objekt ikke er aktivt
        if not self._is_active:
            return
            
        for component in self._components.values():
            if(component._is_active):
                component.update(delt_time) 
              
            
        #opdatere child objekts
        for child in self._children:
            child.update(delt_time)        
         


class GameobjectType(Enum):
    GROUND = 1
    MOVINGPLATFORM = 2
    HAZARDS = 3
    ENEMYSKELETON = 4
    ENEMYGOBLIN = 5
    PLAYER = 6
    GOAL = 7
    
    
    