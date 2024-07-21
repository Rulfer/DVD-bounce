class Collision:
    def __init__(self):
        self.edges = {'left': float('inf'), 'right': float('inf'), 'top': float('inf'), 'bottom': float('inf')}

    def set_edge(self, edge, dis):
        self.edges[edge] = dis

    def get_nearest_edge(self):
        edge = min((x for x in self.edges if self.edges[x] != float('inf')), key=self.edges.get)
        return edge, self.edges[edge]

