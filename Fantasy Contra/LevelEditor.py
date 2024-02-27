import os
import json
import pygame
from Components import SpriteRenderer, Component
import copy
from GameObject import GameobjectType
from EditorBuilder import EditorBuilder
from EditorInput import EditorInput

class LevelEditor:
    
    
    grid_size = 80
    
    
    def __init__(self, gameworld):
        self._gameworld = gameworld
        
        self._levels = [[]]
        #self._scene_amount = 5
        self._scene = 0
        self._level_amount = 10
        
        #de gameobjects man kan placere
        self._gameobjects = []
        self._index = 0
        self._object_offset = pygame.Vector2(0,0)
        
        #filepath for save
        self._filepath = "levels.json"

        self._filepaths = ["level1.json", "level2.json", "level3.json", "level4.json", "level5.json"]
        
        #input
        self._giving_consoleinput = False
        self._input = EditorInput(self)
        
        #temp variabler
        self._last_dictionary = None
        #det sidste gameobjekt der blev tilføjet
        self._last_gameobject = None
        
        #mouspos
        self._mouse_pos = pygame.Vector2(0,0)
        self._snapped_mouse_pos = pygame.Vector2(0,0)
        
        
        
    @property
    def selected_gameobject(self):
        return self._gameobjects[self._index]
    
    @property
    def selected_spriterenderer(self):
        #hvis den har ingen børn så returnere den nuværende
        if(len(self._gameobjects[self._index]._children) == 0):
            return self._gameobjects[self._index].get_component("SpriteRenderer")
        #hvis den har børn så returnere den det første fundet spriterenderer
        else:
            sr_list = self._gameobjects[self._index].get_component_in_children("SpriteRenderer")
            return sr_list[0]
        
        
    #tilføjer et nyt gameobject til editoren og loader sprites
    def add_gameobject(self, gameobject):
            self._gameobjects.append(gameobject)
            gameobject.is_active = False
            gameobject.awake(self._gameworld)
            gameobject.start()
            
    
    def awake(self):
        eb = EditorBuilder(self._gameworld)
        for type in GameobjectType:
            go = eb.build(type, None)
            self.add_gameobject(go)
       
    #setup når editoren starter        
    def start(self):
        self.selected_gameobject.is_active = True

    def late_start(self):
        #læser levels filen for level data
        level_data = self.read_level_data()
        
        
        #opstiller level filens data som en 2D liste
        if(level_data is not None):
            self.instantiate_saved_gameobjects(level_data)
        else:
            self.add_empty_levels()
        
        #loader gameobjects ind for det nuværende level
        self.load_menu()
         
    
    #tilføjer alle gemte objekter til gameworld
    def instantiate_saved_gameobjects(self, level_data):
        eb = EditorBuilder(self._gameworld)
        new_levels_list = []
        levels_loaded = 0
        
        
        for x in level_data:
            
            levels_loaded +=1
            new_row = []
            
            for y in x:
                for z in y:
                    for h in z:
                        if(len(h)==0):
                            continue
                        else:
                            #bygger object ud fra leveldata
                            
                            go = eb.build(h['gameObject'], h)
                            
                            #makes dictionary to save
                            new_dict = h
                            new_dict['gameObject'] = go
                            
                            # adds to lists
                            new_row.append(new_dict)
                            #self._levels[x][y].append(new_dict)
            new_levels_list.append(new_row)
        
        self._levels = new_levels_list
        
        #tilføjer tomme lister for pladsen på de manglende level data
        print(f"{self._level_amount}-{levels_loaded}")
        for missing_level in range(self._level_amount-levels_loaded):
            self._levels.append([])
            print("adding missing level")
            
    def add_empty_levels(self):
        #tilføjer tomme lister for pladsen på de manglende level data
        print(f"{self._level_amount}")
        for missing_level in range(self._level_amount):
            self._levels.append([])
            print("adding missing level")
    
    def serialize_enum(self, enum_member):
        return enum_member.value 
        
    def deserialize_enum(self, enum_class, value):
        return enum_class(value)
    
    def serialize_level(self, x):
        #serialized_list = copy.deepcopy(self._levels)
        
        serialized_list =[]
    
        new_row = []
        for y in x: 
            if(len(y)==0):
                continue
            else:
                #får enum af den nuværende element
                enum = self.serialize_enum(y['gameObject'].type)
                    
                #laver ny dictionarie for at undgår reference type
                new_dict = {
                    'gameObject': enum,
                    'positionX': y['positionX'],
                    'positionY': y['positionY'] 
                    }
                    
                #tilføjer dictionary til liste
                new_row.append(new_dict)
        #tilføjer list af dictionary til liste
        serialized_list.append(new_row)
        
        
        return serialized_list
    
    def deserialize_levels(self, data):
        for x in data:
            for y in x:
                for z in y:
                    z['gameObject'] = self.deserialize_enum(GameobjectType, z['gameObject'])
        return data
        
    #gemmer levelets layout 
    def write_level_data(self, level_data, level):
        self._filepath = self._filepaths[level]
        #file alredy exists
        if os.path.exists(self._filepath):
            with open(self._filepath, "w") as file:
                json.dump(level_data, file)    
                    
            #file doesnt exist yet
        else:
            with open(self._filepath, "w") as file:
                json.dump(level_data, file)    
    
    def save_levels(self):
      
        for i in range(5):
            serialized_data = []
            serialized_data.append(self.serialize_level(self._levels[i]))
            self.write_level_data(serialized_data, i)  
        
    #læser levelets layout
    def read_level_data(self):
        #checker om level filen allerede existere
        deserilaized_data = []
        for i in range(5):
            self._filepath = self._filepaths[i]
            if os.path.exists(self._filepath):
                with open(self._filepath, "r") as file:
                    data = json.load(file)
                    deserilaized_data.append(self.deserialize_levels(data))
                    
            else: 
                print("no data to load")
        return deserilaized_data
    
    #tegner det nuværende selected object ved musens position
    def draw_selected_gameobject_at_cursor(self):
        global grid_size
        #self.selected_sprite.rect.topleft = self.selected_gameobject.transform.position
        
        
        self.selected_gameobject.transform.position = self._snapped_mouse_pos + self._object_offset
        self.selected_spriterenderer.update(0)
    
    def save_object(self, data):
        self._levels[self._scene].append(data) 
        self.save_levels()
        
    
    #kan ikke place hvis man sætter på samme sted som før
    def can_place(self):
        placable = True
        
        dict = self._last_dictionary
        if(dict is None):
            return placable
        
        if(dict['positionX'] == self.selected_gameobject.transform.local_position[0]):
            if(dict['positionY'] == self.selected_gameobject.transform.local_position[1]):
                placable = False
                
        if(self.selected_gameobject.type is GameobjectType.PLAYER):
            for dicts in self._levels[self._scene]:
                obj = dicts['gameObject']
                if(obj.type is GameobjectType.PLAYER):
                    placable = False
        
        
        return placable
        
    #placere en kopi af det objekt der er valgt  
    #gemmer kopi gameobjectets data i levels listen
    def place_selected_gameobject(self):
        mouse_buttons = pygame.mouse.get_pressed()

        if(self.can_place()):
        # Check if the right mouse button is clicked
            if mouse_buttons[0]:
                #denne if sætning er der kun fordi jeg ikke gad fixe indents :)
                if(True):
                    #go = self.selected_gameobject.get_copy(self.selected_gameobject, self._gameworld)
                    
                    ##########################
                    #default værdier der altid er med et object
                    data_dictionary ={
                    'gameObject': self.selected_gameobject,
                    'positionX': self.selected_gameobject.transform.local_position[0],
                    'positionY': self.selected_gameobject.transform.local_position[1]
                    }
                    
                    #tilføjer extra data til det placeret objekt hvis components har mere data at tilføje
                    
                    for component in self.selected_gameobject.components:
                        
                        if(issubclass(self.selected_gameobject._components[component].__class__, Component)):
                            
                            if(self.selected_gameobject._components[component].value() is not None):
                                data_dictionary.update(self.selected_gameobject._components[component].value())
                                
                                #hvis der skal indtastes mere data når den lave
                                self._giving_consoleinput = True
                       
                       
                         
                    
                    #makes the gameobject that is seen in gameworld
                    eb = EditorBuilder(self._gameworld)
                    go = eb.build(self.selected_gameobject.type, data_dictionary)
                    go.make_editor_object()
                    data_dictionary['gameObject'] = go
                    
                    
                    self._last_gameobject = go
                    self._last_dictionary = data_dictionary
                    
                    if(not self._giving_consoleinput):
                        self._levels[self._scene].append(data_dictionary) 
                    
                    go.awake(self._gameworld)
                    self._gameworld._gameObjects.append(go)  
                    
                    if(not self._giving_consoleinput):
                        self.save_levels()
                    
                    
                    
                    
                    
                      
       
    #skifter hvad objekt der er valgt som skal placeres 
    def switch_selected_gameobject(self):
        #finder den key der bliver trykket på 1.2.3.4...0
        
        
            
        # Get the state of all keyboard keys
        keys = pygame.key.get_pressed()

        # Check if a number key (0-9) is pressed
        for i in range(10):
            if keys[pygame.K_0 + i]:
                number = i
                self.selected_gameobject.is_active = False          
                    #checker om det ude af range
                if(number<=len(self._gameobjects)):
                    self._index = number-1
                else: 
                    self._index = len(self._gameobjects) -1
                        
                self.selected_gameobject.is_active = True
                    
    def take_input(self):
        self._input.take_input(self.selected_gameobject.type)
     
    def move_camera(self):
        pass
    
    def erase_gameobject(self):
        
        #laver et rect ud fra musens position og sletter objekt der er ramt af et 5x5 pixels rect
        mous_rect = pygame.Rect(self._mouse_pos[0],self._mouse_pos[1], 5,5)
        obj_list = self._levels[self._scene]
        
        #går igennem alle gameworlds colliders
        for collider in self._gameworld._colliders:
            #checker om der er en collision
            is_rect_colliding = collider._collision_box.colliderect(mous_rect)
            #hvis der er en collision så slet objekt
            if(is_rect_colliding):
                erasing_gameobject = collider.gameObject
                
                print(erasing_gameobject.type)
                #finder objektet i listen over level data
                for dict in obj_list:
                    
                    if(dict['gameObject'] is erasing_gameobject):
                        #destroys gameobject in gameworld
                        erasing_gameobject.destroy()
                        #removes dictionary from list
                        self._levels[self._scene].remove(dict)
                        self.save_levels()
                        print("erase stuff")
                        
        
        
        #backend delete
        #go through current loaded scene and find object with same pos and deletes that object from saved data
        #obj_list = self._levels[self._scene]
        #dictionary_to_remove = None
        
        #for dict in obj_list:
            #if(dict['positionX'] == self._snapped_mouse_pos.x):
                #if(dict['positionY'] == self._snapped_mouse_pos.y):
                    #dictionary_to_remove = dict
        
        #if(dictionary_to_remove is not None):
            #obj_list.remove(dictionary_to_remove)
        #else:
            #print("no object to remove")
                    
        #pass
    
    #hvis højre museknap trykkes så kald slet object funktionen
    def eraser(self):
        mouse_buttons = pygame.mouse.get_pressed()

        # Check if the right mouse button is clicked
        if mouse_buttons[2]:
            self.erase_gameobject()
           
    def update(self):
        
        
        #tager imod input istedet for andre funktioner hvis input er i gang
        if(self._giving_consoleinput):
            self.take_input()
            #returnere så andre funktioner ikke bliver kaldt
            return
        
        #sætter musepositionen fra spiller som variable
        self._mouse_pos = pygame.mouse.get_pos() + self._gameworld.camera_offset
        self._snapped_mouse_pos = (round(self._mouse_pos[0] / self.grid_size) * self.grid_size, round(self._mouse_pos[1] / self.grid_size) * self.grid_size)
        
        #tegn et gameobject ved cursor
        #gameobjectet snapper til et grid
        self.draw_selected_gameobject_at_cursor()
        
        #man kan placere en kopi af det gameobject der holdes
        #man kan save levelet
        self.place_selected_gameobject()
        
        #man kan skifte hvad object der bliver holdt
        self.switch_selected_gameobject()
        
        #bevæge kamera rundt / zoom
        self.move_camera()
        
        #eraser tool til at slette placeret ting
        self.eraser()
    
    
    #loader et nyt givent level
    def change_level(self, level_index):
        
        #unloads level
        self.unload_level()
        
        #changes level index
        self._scene = level_index
        
        #hvis der er et level at loade så load det
        
        if(len(self._levels[self._scene])>0):
            #loads new level in
            self.load_level()
        else:
            print("der er ikke noget at loade ind")
        
        #informere gameworld at et level er blevet loadet
        self._gameworld.level_loaded()
        
        
    #skifter til det næste level efter det nuværende
    def next_level(self):
        self.change_level(self._scene+1)
    
    def load_menu(self):
        #unloader level
        self.unload_level()
        #ændre scene til 0
        self._scene = 0
        #loader det level
        self.load_level()
        #fortæller gameworld at menu bliver loadet
        self._gameworld.menu_loaded()  
    
    def restart_level(self):
        self.change_level(self._scene)
    
    #unloader det nuværende levels gameobjekts
    def unload_level(self):
        for dict in self._levels[self._scene]:
            obj = dict['gameObject']
            spawnpos = pygame.Vector2(
                dict['positionX'],
                dict['positionY'],
            )
            obj.transform.position = spawnpos
            
            #obj.make_not_editor_object()
            if(obj in self._gameworld._gameObjects):
                self._gameworld._gameObjects.remove(obj)
                #print("removing object from scene")
            
                
                
                
                
            #self._gameworld._gameObjects = []
    
    #loader det nuværende levels gameobjekts
    def load_level(self):
        for dict in self._levels[self._scene]:
            obj = dict['gameObject']
            spawnpos = pygame.Vector2(
                dict['positionX'],
                dict['positionY'],
            )
            obj.transform.position = spawnpos
            self._gameworld.instantiate(obj)
            
            #hvis det er i editormode så disable de andre components som ikke er spriterendere
            if(self._gameworld._editormode):
                obj.make_editor_object()
            
            
        
        
        
            