require_relative 'route'
require_relative 'city'

class Population
    attr_accessor :routes, :best, :second_best
    
    def initialize(routes)
        @routes = routes
        sort_routes
    end

    def crossover
        i, j = generate_random_index

        best_copy = @best.cities.dup
        second_best_copy = @second_best.cities.dup
        
        # Generate offspring with that keeps i..j, but change remaining element to nil
        # Example:
        #       [a, b, c, d, e, f, g]             # original
        #       [nil, b, c, d, nil, nil, nil]     # i = 1, j = 3
        # 
        @offspring_a = Route.new(best_copy.map.with_index {|c, index| (i..j) === index ? c : nil })
        @offspring_b = Route.new(second_best_copy.map.with_index {|c, index| (i..j) === index ? c : nil })

        # Array to be inserted to empty spaces of another array
        arr_a = best_copy - second_best_copy[i..j]
        arr_b = second_best_copy - best_copy[i..j]
        
        @offspring_a.cities.map! do |city|
            city || arr_b.shift
        end

        @offspring_b.cities.map! do |city|
            city || arr_a.shift
        end

        @routes.concat([@offspring_a, @offspring_b])
    end

    def select
        sort_routes
    end

    # Generate two random index, swap if i > j
    def mutate
        i, j = rand(City.cities.size), rand(City.cities.size)
        if i > j
            @offspring_a.cities[i], @offspring_a.cities[j] = @offspring_a.cities[j], @offspring_a.cities[i]
        end

        i, j = rand(City.cities.size), rand(City.cities.size)
        if i > j
            @offspring_b.cities[i], @offspring_b.cities[j] = @offspring_b.cities[j], @offspring_b.cities[i]
        end
    end

    # After inserting two new offspring, sort routes and remove the one with least fitness
    def remove_weakest_individual
        sort_routes
        @routes = @routes[0..-2]
    end

    def self.generate_population(count)
        arr = Array.new(count) do
            Route.randomize(City.cities)
        end

        Population.new(arr)
    end

    private

    def sort_routes
        @routes.sort_by!(&:fitness).reverse!

        @best = @routes[0]
        @second_best = @routes[1]
    end

    def generate_random_index
        i, j = 0, 0
        while i == j || (i - j).abs == City.cities.length-1
            i = rand(City.cities.length)
            j = rand(City.cities.length)
        end

        i, j = j, i if i > j

        return i, j
    end
end