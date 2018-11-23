require 'byebug'

require_relative 'city'
require_relative 'route'
require_relative 'population'

City.init

pop = Population.generate_population(20)
generation = 0

until pop.routes[0].fitness - pop.routes[-1].fitness < 0.00000000001 do
    # Select two routes with highest fitness
    pop.select

    # Produce two offspring from the highest two routes
    # and add it back to list of routes
    pop.crossover

    # Mutate the offspring randomly 
    pop.mutate

    # Remove route with least fitness
    pop.remove_weakest_individual
    generation += 1
end

# pp pop.routes.map(&:distance)
# p "Diff: #{pop.best.fitness - pop.second_best.fitness}"

puts "Generation : #{generation}"
p "=" * 99
# p "Fitness: #{pop.best.fitness}"
p "Distance: #{pop.best.distance}"
p "Route:"
pp pop.best.cities

