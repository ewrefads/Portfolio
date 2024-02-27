import pygame
from Components import dir_prefix
Asset_directory = dir_prefix + 'Assets/'
class Background:
    def __init__(self, screen_width, screen_height, camera):
        self.screen_width = screen_width
        self.screen_height = screen_height
        self.camera = camera
        self.background_far = pygame.image.load(Asset_directory + "background_far.png").convert_alpha()
        self.background_mid = pygame.image.load(Asset_directory + "background_mid.png").convert_alpha()
        self.background_near = pygame.image.load(Asset_directory + "background_near.png").convert_alpha()

        self.background_far = pygame.transform.scale(self.background_far, (screen_width, screen_height))
        self.background_mid = pygame.transform.scale(self.background_mid, (screen_width, screen_height))
        self.background_near = pygame.transform.scale(self.background_near, (screen_width, screen_height))
        #initial scroll position
        self.scroll_far = 0
        self.scroll_mid = 0
        self.scroll_near = 0

        self.scroll_speed_far = 1
        self.scroll_speed_mid = 2
        self.scroll_speed_near = 4

    #test scroll for background loop
    # def scroll(self):
    #     # Update scroll positions based on scroll speeds
    #     self.scroll_far -= self.scroll_speed_far
    #     self.scroll_mid -= self.scroll_speed_mid
    #     self.scroll_near -= self.scroll_speed_near

    def scroll(self):
        # Update scroll positions based on camera position
        camera_offset = self.camera.transform.position.x
        self.scroll_far = -(camera_offset * 0.25)
        self.scroll_mid = -(camera_offset * 0.5)
        self.scroll_near = -camera_offset
        
        if self.scroll_far <= -self.screen_width:
            self.scroll_far = 0
        if self.scroll_mid <= -self.screen_width:
            self.scroll_mid = 0
        if self.scroll_near <= -self.screen_width:
            self.scroll_near = 0

    def render(self, window):
        pass
        window.blit(self.background_far, (self.scroll_far, 0))
        window.blit(self.background_far, (self.scroll_far + self.screen_width, 0))
        window.blit(self.background_mid, (self.scroll_mid, 0))
        window.blit(self.background_mid, (self.scroll_mid + self.screen_width, 0))
        window.blit(self.background_near, (self.scroll_near, 0))
        window.blit(self.background_near, (self.scroll_near + self.screen_width, 0))
