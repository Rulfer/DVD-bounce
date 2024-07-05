from tkinter import *
import random


class my_label:
    def __init__(self, tk, img):
        self.xPos = random.randrange(start=0, stop=tk.winfo_screenwidth() - img.width())
        self.yPos = random.randrange(start=100, stop=tk.winfo_screenheight() - img.height())
        self.label = Label(
            master=tk,
            image=img,
            bg="black",
            width=img.width(),
            height=img.height()
        )
        self.label.place(x=0, y=0)
        self.label.pack()
        self.img_width = img.width()
        self.img_height = img.height()
        self.go_right = random.choice([True, False])
        self.go_up = random.choice([True, False])

    def update(self):
        self.label.place(x=self.xPos, y=self.yPos)

    def horizontal_modifier(self):
        return 1 if self.go_right else -1

    def vertical_modifier(self):
        return -1 if self.go_up else 1

    def horizontal_hit(self):
        self.go_right = not self.go_right

    def vertical_hit(self):
        self.go_up = not self.go_up

    def get_borders(self):
        # Border of the image
        xPos = self.xPos

        # First, get corner to the right
        upperLeft = self.xPos, self.yPos
        lowerRight = self.xPos + self.img_width, self.yPos + self.img_height
        return upperLeft, lowerRight
