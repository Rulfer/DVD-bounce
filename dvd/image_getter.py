from tkinter import *
from PIL import Image, ImageTk


def get_image(active_tkinter):
    img = Image.open("images/ODDBALL.png").convert("RGBA")
    width, height = img.size
    left = 125
    top = 20
    right = width - 100
    bottom = height - 20

    border = (left, top, right, bottom)
    cropped_image = img.crop(border)

    #if cropped_image.mode != 'RGBA':
    #    cropped_image = cropped_image.convert('RGBA')

    #transparent_background = Image.new('RGBA', cropped_image.size, (0, 0, 0, 0))
    #transparent_background.paste(cropped_image, (0, 0), cropped_image)

    tk_image = ImageTk.PhotoImage(image=cropped_image, master=active_tkinter)
    return tk_image
