require_relative 'city'

class Route
    attr_accessor :cities

    def initialize(cities)
        @cities = cities
    end

    def distance
        @distance ||= @cities.zip(@cities.rotate).reduce(0) do |total, pair|
            total + City.distances[pair[0]][pair[1]] 
        end
    end

    def fitness
        @fitness ||= 1.0 / distance
    end

    def self.randomize(cities)
        Route.new(cities.shuffle)
    end

end