// Recipe Ingredient Management
// Handles dynamic adding/removing of ingredient rows in recipe forms

let ingredientIndex = 0;

/**
 * Adds a new ingredient row to the form
 * @param {string} pageHandler - The page handler URL (e.g., '/Recipes/Create' or '/Recipes/Edit')
 */
async function addIngredient(pageHandler) {
    const container = document.getElementById('ingredients-container');
    const noIngredientsMsg = document.getElementById('no-ingredients-message');

    try {
        // Fetch new ingredient row from server
        const response = await fetch(`${pageHandler}?handler=AddIngredientRow&index=${ingredientIndex}`);

        if (!response.ok) {
            throw new Error('Failed to load ingredient row');
        }

        const html = await response.text();

        // Add the new row
        container.insertAdjacentHTML('beforeend', html);

        // Hide the "no ingredients" message
        noIngredientsMsg.style.display = 'none';

        ingredientIndex++;
    } catch (error) {
        console.error('Error adding ingredient:', error);
        alert('Failed to add ingredient. Please try again.');
    }
}

/**
 * Updates ingredient indices after removal
 * Ensures form field names are sequential
 */
function updateIngredientIndices() {
    const container = document.getElementById('ingredients-container');
    const rows = container.querySelectorAll('.ingredient-row');
    const noIngredientsMsg = document.getElementById('no-ingredients-message');

    // Show/hide "no ingredients" message
    if (rows.length === 0) {
        noIngredientsMsg.style.display = 'block';
    } else {
        noIngredientsMsg.style.display = 'none';
    }

    // Update indices to be sequential
    rows.forEach((row, index) => {
        row.id = `ingredient-${index}`;
        row.querySelectorAll('input, select').forEach(input => {
            const name = input.getAttribute('name');
            if (name) {
                input.setAttribute('name', name.replace(/\[\d+\]/, `[${index}]`));
            }
        });
    });

    ingredientIndex = rows.length;
}

/**
 * Initializes ingredient count on page load
 * Call this when editing an existing recipe with ingredients
 */
function initializeIngredientIndex() {
    const container = document.getElementById('ingredients-container');
    const rows = container.querySelectorAll('.ingredient-row');
    ingredientIndex = rows.length;

    // Hide "no ingredients" message if there are ingredients
    if (rows.length > 0) {
        const noIngredientsMsg = document.getElementById('no-ingredients-message');
        noIngredientsMsg.style.display = 'none';
    }
}
