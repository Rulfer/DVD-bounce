from tkinter import *
import time
import settings
from PIL import Image, ImageTk
from image_getter import get_image
import animated_label


class DVD:
    def __init__(self):
        # tkinter
        self.tk = Tk()

        # Get screen width and height
        self.screen_width = self.tk.winfo_screenwidth()
        self.screen_height = self.tk.winfo_screenheight()

        # Initialize Canvas
        self.canvas = Canvas(self.tk, width=self.screen_width, height=self.screen_height, highlightthickness=0, bg="black")
        self.canvas.pack(anchor=CENTER, expand=True)

        self.tk.overrideredirect(True)
        self.tk.wm_attributes("-topmost", True)
        self.tk.wm_attributes("-disabled", True)
        self.tk.wm_attributes("-transparentcolor", "black")

        # Initialize time
        self.previous_time = time.time()

        # Initialize images
        self.img = get_image(self.tk)
        self.labels = []
        # What are for-loops, am I right?
        self.labels.append(self.create_image())
        self.labels.append(self.create_image())
        self.labels.append(self.create_image())
        self.labels.append(self.create_image())
        self.labels.append(self.create_image())
        self.labels.append(self.create_image())
        self.labels.append(self.create_image())
        self.labels.append(self.create_image())
        self.labels.append(self.create_image())

        self.move_image()

        self.tk.geometry(f'{self.screen_width}x{self.screen_height}')
        self.tk.mainloop()

    def move_image(self):
        current_time = time.time()
        delta_time = current_time - self.previous_time
        self.previous_time = current_time

        for label in self.labels:
            label.xPos += round(settings.xVel * label.horizontal_modifier() * delta_time)
            label.yPos += round(settings.yVel * label.vertical_modifier() * delta_time)

            # Clamp value to within the screen (prevents getting stuck on edges)
            label.xPos = max(min(label.xPos, self.screen_width), 0)
            label.yPos = max(min(label.yPos, self.screen_height), 0)

            label.update()

        self.tk.after(settings.milliseconds, self.move_image)

    def create_image(self):
        label = animated_label.MyLabel(self.tk, self.img, self.canvas)
        return label

    def calculate_image_collisions(self):
        for label in self.labels:
            overlapping_items = self.canvas.find_overlapping(*self.canvas.bbox(label.id))
            my_bbox = self.canvas.bbox(label.id)

            collisions = self.overlapping(label)
            for overlap_item, collision in collisions.items():
                l = self.get_label(collision['id'])

                if collision['right']:
                    # l.set_right(True)
                    label.set_right(False)
                if collision['left']:
                    # l.set_right(False)
                    label.set_right(True)
                if collision['top']:
                    # l.go_up = False
                    label.go_up = True
                if collision['bottom']:
                    # l.go_up = True
                    label.go_up = False


    #def get_overlap_side(self, bbox1, bbox2):
    #    left1, top1, right1, bottom1 = bbox1
    #    left2, top2, right2, bottom2 = bbox2

    #    if right1 > left2 and left1 < right2 and bottom1 > top2 and top1 < bottom2:

    #        overlaps = []
    #        if right1 > left2 and left1 < left2:
    #            overlaps.append('left')
    #        if left1 < right2 and right1 > right2:
    #            overlaps.append('right')
    #        if bottom1 > top2 and top1 < top2:
    #            overlaps.append('top')
    #        if top1 < bottom2 and bottom1 > bottom2:
    #            overlaps.append('bottom')
    #        return overlaps
    #    return []

    #def overlapping(self, label):
    #    overlapping_items = self.canvas.find_overlapping(*self.canvas.bbox(label.id))
    #    collisions = {}

    #    my_bbox = self.canvas.bbox(label.id)
    #    for overlap_item in overlapping_items:
    #        if overlap_item != label.id:
    #            # Get the collision between my bbox and the other bbox
    #            other_bbox = self.canvas.bbox(overlap_item)
    #            #collisions[overlap_item] = self.determine_edges(my_bbox, other_bbox, overlap_item)
    #            collisions[overlap_item] = self.simple_edge(my_bbox, other_bbox, overlap_item)


    #   return collisions

    def simple_edge(self, my_bbox, other_bbox, id):
        edges = {'id': id, 'left': False, 'right': False, 'top': False, 'bottom': False}

        left1, top1, right1, bottom1 = my_bbox
        left2, top2, right2, bottom2 = other_bbox

        if right1 >= left2 > left1:
            edges['right'] = True
        if left1 <= right2 < right1:
            edges['left'] = True
        if bottom1 >= top2 > top1:
            edges['bottom'] = True
        if top1 <= bottom2 < bottom1:
            edges['top'] = True

        return edges

    def determine_edges(self, bbox0, bbox1, id):
        edges = {'id': id, 'left': False, 'right': False, 'top': False, 'bottom': False}

        #if bbox0[2] >= bbox1[0] and bbox0[0] <= bbox1[2] and bbox0[3] >= bbox1[1] and bbox0[1] <= bbox1[3]:
        # Calculate the distances to each edge
        right_distance = bbox1[0] - bbox0[2]
        left_distance = bbox0[0] - bbox1[2]
        bottom_distance = bbox1[1] - bbox0[3]
        top_distance = bbox0[1] - bbox1[3]

        # Mark edges as True if they overlap
        if right_distance < 0:
            edges['right'] = True
        if left_distance < 0:
            edges['left'] = True
        if bottom_distance < 0:
            edges['bottom'] = True
        if top_distance < 0:
            edges['top'] = True

        # Find the minimum distance to determine the closest edge collision
        distances = {
            'right': right_distance,
            'left': left_distance,
            'bottom': bottom_distance,
            'top': top_distance
        }

        # Get the minimum distance and corresponding edge
        min_edge = min(distances, key=distances.get)

        # Mark only the closest edge as colliding
        if distances[min_edge] != float('inf'):
            edges[min_edge] = True

        return edges

    def _overlapping(self, label):
        x0, y0, x1, y1 = self.canvas.bbox(label.id)
        overlapping_items = self.canvas.find_overlapping(x0, y0, x1, y1)

        if len(overlapping_items) > 1:
            other_label = self.get_label(overlapping_items[1])

            my_coords = self.canvas.coords(label.id)
            other_coords = self.canvas.coords(other_label.id)

            # Ensure they move away horizontally
            label.go_right = True if my_coords[0] < other_coords[0] else False
            other_label.go_right = True if other_coords[0] < my_coords[0] else False

            label.go_up = True if my_coords[1] < other_coords[1] else False
            other_label.go_up = True if other_coords[1] < my_coords[1] else False

    def get_label(self, id_filter):
        return next(x for x in self.labels if x.id == id_filter)


DVD()

