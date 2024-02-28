import pygame
import os
from Components import dir_prefix
music_directory = dir_prefix + 'Assets/Audio/Music'
sfx_directory = dir_prefix + 'Assets/Audio/SFX'


class AudioManager():

    def __init__(self):

        # Set antallet af mixer channels. Default er 8. Experimenter med 16 eller 32 for at finde ud af om det har en stor påvirkning af performance
        pygame.mixer.set_num_channels(32) 

        # Lav en collection af audio. Grunden til at vi gør det er at vi ikke skal bøvle med at skrive directory path for hvert lydeffekt vi vil afspille
        self._audio_dict = self.load_audio_files(music_directory)
        self._sfx_dict = self.load_audio_files(sfx_directory)
        self._is_paused = False
        
        self._volume = 0.5
        

    def play_audio(self, name):
        
        # Afspiller lydfil fra audio_dict (audio dictionary)
        if name in self._audio_dict:
            self._audio_dict[name].play()
            
        # play(loops=0, start=0.0, fade_ms=0)
        # The music repeats indefinitely if this argument is set to -1

    def stop_audio(self, name):
        # Stopper music med navn i param
        if name in self._audio_dict:
            self._audio_dict[name].stop()


    def pause_all_sounds(self):
        if self._is_paused is False:
            pygame.mixer.pause()
            self._is_paused = True
        else:
            self.unpause_all_sounds()
            self._is_paused = False

    def unpause_all_sounds(self):
            pygame.mixer.unpause()
        
    def play_sfx(self, name):
        if name in self._sfx_dict:
            self._sfx_dict[name].play()

    def change_volume(self,amount):
        # Set master volume mellem 0.0 - 1.0. Værdier over 1.0 bliver sat til 1.0
        self._volume+= amount
        print(f"volume is now: {self._volume}")
        #pygame.mixer.set_volume(self._volume)
        for key in self._audio_dict:
            self._audio_dict[key].set_volume(self._volume)
            
    
    def set_volume(self, volume):
        # Set master volume mellem 0.0 - 1.0. Værdier over 1.0 bliver sat til 1.0
        pygame.mixer.music.set_volume(volume)

    def load_audio_files(self, audio_dir):
        audio_dict = {}
        for filename in os.listdir(audio_dir):
            if filename.endswith('.mp3'):  # Adjust file extension as needed
                name = os.path.splitext(filename)[0]  # Get filename without extension
                print(name)
                filepath = os.path.join(audio_dir, filename)
                audio_dict[name] = pygame.mixer.Sound(filepath)
        return audio_dict


    